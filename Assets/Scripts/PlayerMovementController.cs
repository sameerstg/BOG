using Photon.Pun;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public Player player;
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
    bool onWall;
    PhotonView photonView;
    [Space]
    [Space]
    [Space]
    public float wallMoveDownSpeed;
    public float wallMoveDownSpeedLimit;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        if (!photonView.IsMine)
        {
            return;
        }
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        currentJumps = totalJumps;
    }
    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        Movevement();
    }
    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        ClampSpeed();
    }
    internal void SetMoveDirection(Vector2 direction)
    {
        if (direction == this.direction )
        {
            return;
        }
        this.direction = direction;
    }

    private void Movevement()
    {
        if (direction == Vector2.zero)
        {
            return;
        }
        transform.Translate(direction.normalized * moveSpeed * 0.05f);
    }
    void ClampSpeed()
    {
        if (rb.velocity.y > jumpMaxSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, Vector2.ClampMagnitude(rb.velocity, jumpMaxSpeed).y);
        }
        if (rb.velocity.y < 0 && rb.gravityScale == 0)
        {
            if (rb.velocity.y < -wallMoveDownSpeedLimit)
            {
                rb.velocity = new Vector2(rb.velocity.x, Vector2.ClampMagnitude(rb.velocity, wallMoveDownSpeedLimit).y);
            }
        }

    }
    public void Jump()
    {
        if (currentJumps <= 0)
        {
            return;
        }
        rb.AddForce(Vector2.up * jumpPower * 0.2f, ForceMode2D.Impulse);
        currentJumps -= 1;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (collision.collider.CompareTag("Land"))
        {

            currentJumps = totalJumps;

        }
        if (collision.collider.CompareTag("Wall"))
        {
            onWall = true;
            currentJumps = totalJumps;
            rb.gravityScale = 0;


        }
        if (collision.collider.CompareTag("Player"))
        {
            //Debug.Log("player");
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (collision.collider.CompareTag("Wall"))
        {
            onWall = true;
            //rb.AddForce(Vector2.down * wallMoveDownSpeed, ForceMode2D.Impulse);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        rb.gravityScale = 1;
        onWall = false;
    }
  

}
