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
    public void SetDirection(bool flip,Vector2 position)
    {
        photonView.RPC(nameof(SetDirectionRPC), RpcTarget.All, new object[] { flip, position });
    }
    [PunRPC]
    public void SetDirectionRPC(bool flip,Vector2 pos)
    {
        spriteRenderer.flipX= flip;
        transform.position = pos;
    }
    public void Equip(string id)
    {
        photonView.RPC(nameof(EquipRPC), RpcTarget.AllBufferedViaServer,id);
    }
    [PunRPC]
    public void EquipRPC(string id)
    {
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponent<BoxCollider2D>());
        Destroy(GetComponent<CircleCollider2D>());
        transform.parent = RoomManager._instance.players.Find(p => p.id == id)?.player.transform;
      
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
