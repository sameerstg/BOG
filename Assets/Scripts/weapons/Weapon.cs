using Photon.Pun;
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
        //if (flip == spriteRenderer.flipX)
        //{
        //    return;
        //}
        photonView.RPC(nameof(SetDirectionRPC), RpcTarget.All, new object[] { flip, position });
    }
    [PunRPC]
    public void SetDirectionRPC(bool flip,Vector2 pos)
    {
        spriteRenderer.flipX= flip;
        transform.position = pos;
    }
}
