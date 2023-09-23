using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponManager manager;
    string weaponName;
    public SpriteRenderer spriteRenderer;
    public PhotonView photonView;
    public Player player;
    public int bulletInMag,totalBullets;
    bool isReloading;
    bool isFiring;
    float lastFireTime;
    Vector2 bulletDirection;
    float bulletLifetime;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        photonView = GetComponent<PhotonView>();
        weaponName = manager.name.Split(' ')[0]; 
        totalBullets = manager.totalBullet;
        bulletInMag = manager.bulletPerMag;
        bulletLifetime = manager.bulletLifetime;
    }
    internal void Fire(Vector2 bulletDirection)
    {
        this.bulletDirection = bulletDirection;
        if (manager.isAutomatitc)
        {
            StartCoroutine(Firing());
        }
        else if (Time.time > lastFireTime + manager.fireRate || bulletInMag == manager.bulletPerMag)
        {
            SpawnBullet();
            if (bulletInMag <= 0)
            {
                StartCoroutine(ReloadDelay());
            }
            
        }

       
    }
    IEnumerator ReloadDelay()
    {
        if (totalBullets >0)
        {
            isReloading = true;
            yield return new WaitForSeconds(manager.reloadingTime);
            Reload();
            isReloading = false;
        }

    }
    void Reload()
    {
        if (totalBullets >= manager.bulletPerMag)
        {
            totalBullets -= manager.bulletPerMag;
            bulletInMag = manager.bulletPerMag;
        }
        else
        {
            totalBullets = 0;
            bulletInMag = totalBullets;
        }
       player?.UpdateWeaponInfo(weaponName,bulletInMag,totalBullets);
    }
    void SpawnBullet()
    {
        Vector2 spawnPosition = new Vector2(transform.position.x + (player.playerDirection.x * 1.1f), transform.position.y + (player.playerDirection.y * 1.1f));
        GameObject bulletObject = PhotonNetwork.Instantiate("BulletMedium", spawnPosition, Quaternion.LookRotation(player.playerDirection, Vector2.up), 0, new object[] { bulletDirection });
        bulletObject.GetComponent<Projectile>().lifetime = bulletLifetime;
        bulletInMag--;
        lastFireTime = Time.time;
        player.UpdateWeaponInfo(weaponName, bulletInMag, totalBullets);
    }

    IEnumerator Firing()
    {
        isFiring = true;
        while (player.playerInputController.playerActions.Attack1.IsInProgress()  && bulletInMag>0)
        {
            SpawnBullet();
            yield return new WaitForSeconds(manager.fireRate);
        }
        isFiring = false;

        if (bulletInMag<=0)
        {
            StartCoroutine(ReloadDelay());
        }
    }
    public void Equip(string id,Vector2 playerDirection)
    {
        photonView.RPC(nameof(EquipRPC), RpcTarget.AllBufferedViaServer,id);
    }
    [PunRPC]
    public void EquipRPC(string id)
    {
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponent<BoxCollider2D>());
        Destroy(GetComponent<CircleCollider2D>());
        PlayerDetails playerDetail = RoomManager._instance.players.Find(p => p.id == id);
        player = playerDetail.player.GetComponent<Player>();
        if (player == null)
        {
            return;
        }
        var hand = player.rightHand;
        transform.position = (Vector2)hand.transform.position + (Vector2.right * manager.weaponOffset);
        transform.SetParent(hand.transform);
        player.UpdateWeaponInfo(weaponName, bulletInMag, totalBullets);
    }
    public void Destroy()
    {
        photonView.RPC(nameof(DestroyRPC), RpcTarget.AllBufferedViaServer);
    }
    [PunRPC]
    public void DestroyRPC()
    {
        WeaponGenerator._instance.allGenerated.Remove(gameObject);
        Destroy(gameObject);
    }
}
