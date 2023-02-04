using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float moveSpeed;
    Vector2 direction = Vector2.zero;
    [SerializeField] Rigidbody2D rb;
    public int damage;

    public void SetDirection(Vector2 dir)
    {
        rb.velocity = dir * moveSpeed;
    }
       private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
