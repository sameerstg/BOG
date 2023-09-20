using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponManager manager;
    public SpriteRenderer spriteRenderer;
    public PhotonView photonView;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        photonView = GetComponent<PhotonView>();
    }
      public void Equip(string id,Vector2 playerDirection)
    {
        photonView.RPC(nameof(EquipRPC), RpcTarget.AllBufferedViaServer,id, playerDirection);
    }
    [PunRPC]
    public void EquipRPC(string id, Vector2 playerDirection)
    {
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponent<BoxCollider2D>());
        Destroy(GetComponent<CircleCollider2D>());
        PlayerDetails player = RoomManager._instance.players.Find(p => p.id == id);
        if (player == null)
        {
            return;
        }
        var body = player.player.GetComponentInChildren<SpriteRenderer>().gameObject;
        var bodyScale = body.transform.localScale;
        body.transform.localScale= new Vector3(1,1,1);
        transform.position = (Vector2)body.transform.position + (Vector2.right * manager.weaponOffset);
        transform.parent = body.transform;
        body.transform.localScale = bodyScale;
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
