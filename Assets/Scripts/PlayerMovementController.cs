using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    protected Rigidbody2D rb;

    public float moveSpeed;
    public float moveMaxSpeed;
    [Space]
    [Space]
    [Space]
    public float jumpPower;
    public float jumpMaxSpeed;
    public int totalJumps;
    public int currentJumps;
    internal Vector2 direction;
    bool canWallMoveDown;
    PhotonView photonView;
    private Vector3 remotePosition;
    [Space]
    [Space]
    [Space]
    public float wallMoveDownSpeed;
    public float wallMoveDownSpeedLimit;

    public Vector2 lookingDirection;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
    }
    private void Start()
    {
        currentJumps = totalJumps;
    }
    private void FixedUpdate()
    {
        Movevement();


    }
    private void Update()
    {


        if (rb.velocity.SqrMagnitude() >= moveMaxSpeed && photonView.IsMine)
        {
            rb.velocity = new Vector2(Vector2.ClampMagnitude(rb.velocity, moveMaxSpeed).x, rb.velocity.y);
        }
        if (rb.velocity.y > jumpMaxSpeed)
        {

            rb.velocity = new Vector2(rb.velocity.x, Vector2.ClampMagnitude(rb.velocity, jumpMaxSpeed).y);
        }
        if (rb.velocity.y <0 && rb.gravityScale ==0)
        {
            if (rb.velocity.y<-wallMoveDownSpeedLimit)
            {
                rb.velocity = new Vector2(rb.velocity.x, Vector2.ClampMagnitude(rb.velocity, wallMoveDownSpeedLimit).y);

            }
        }
    }

    private void Movevement()
    {
        if (direction == Vector2.zero || !photonView.IsMine)
        {
            return;
        }
        rb.AddForce(direction * moveSpeed, ForceMode2D.Impulse);
        lookingDirection = direction.normalized;
    }
    public void Jump()
    {
        if (currentJumps <= 0)
        {
            return;
        }
        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        currentJumps -= 1;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Land"))
        {

            currentJumps = totalJumps;

        }
        if (collision.collider.CompareTag("Wall"))
        {
            currentJumps = totalJumps;
            rb.gravityScale = 0;


        }
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("player");
        }
        
        


    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            canWallMoveDown = true;
            rb.AddForce(Vector2.down * wallMoveDownSpeed, ForceMode2D.Impulse);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        rb.gravityScale = 1;
        canWallMoveDown = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            transform.position = new Vector2();
        }
    }
    internal void SetMoveDirection(Vector2 direction) => this.direction = direction;

}
