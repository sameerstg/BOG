using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    PhotonView photonView;
    PlayerMovementController playerMovementController;
    PlayerAttackGenerater attackGenerator;
    public float weaponOffset;
    Vector2 playerDirection = Vector2.right;
    public Health health;
    Slider slider;
    [SerializeField] float fireRate = 0.5f;
    float nextFire = 0.0f;
    public List<Weapon> weapons = new();
    public Weapon currentWeapon;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        RoomManager._instance.players.Add(new PlayerDetails((string)photonView.InstantiationData[0],(string) photonView.InstantiationData[1],gameObject));
        RoomManager._instance.targetGroup.AddMember(transform,1,3);
        playerMovementController = GetComponent<PlayerMovementController>();
        attackGenerator = GetComponent<PlayerAttackGenerater>();
        slider = GetComponentInChildren<Slider>();
    }
 
    private void OnEnable()
    {
        health.OnGetAttack += GetAttack;
    }
    private void OnDisable()
    {
        playerMovementController = null;
        health.OnGetAttack -= GetAttack;
    }

   


    private void GetAttack()
    {
        slider.value = health.currentHealth / health.totalHealth;
    }

    public void Fire(InputAction.CallbackContext obj)
    {
        if (Time.time > nextFire && currentWeapon != null)
        {
            nextFire = Time.time + currentWeapon.manager.fireRate;
            attackGenerator.Fire(playerMovementController.direction, playerDirection);
        }
    }

    public void Jump(InputAction.CallbackContext obj)
    {
        playerMovementController.Jump();
    }
    public void Move(InputAction.CallbackContext obj)
    {
        playerMovementController.SetMoveDirection(obj.ReadValue<Vector2>());
        if (obj.ReadValue<Vector2>() != Vector2.zero)
        {
            bool shouldStreamDirectionOfWeapon = !(playerDirection == obj.ReadValue<Vector2>());
            playerDirection = obj.ReadValue<Vector2>();
            SetWeaponTransform(shouldStreamDirectionOfWeapon);
        }
    }
    void SetWeaponTransform(bool streamDirectionOfWeapon)
    {
        if (currentWeapon != null)
        {
            if (playerDirection.x > 0)
            {
                currentWeapon.transform.position = transform.position + Vector3.right * weaponOffset;
                if (streamDirectionOfWeapon)
                {
                    currentWeapon.SetDirection(false);
                }
            }
            else
            {
                currentWeapon.transform.position = transform.position + Vector3.left * weaponOffset;
                if (streamDirectionOfWeapon)
                {
                    currentWeapon.SetDirection(true);
                }
            }

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.collider.CompareTag("Bullet"))
        {
            health.GetDamage(collision.collider.GetComponent<Projectile>().damage);
        }
        else if (collision.collider.CompareTag("Weapon"))
        {
            currentWeapon?.gameObject.SetActive(false);
            collision.collider.tag = "Untagged";
            collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            collision.gameObject.GetComponent<Rigidbody2D>().simulated = false;

            currentWeapon = collision.gameObject.GetComponent<Weapon>();
            currentWeapon.transform.parent = transform;
            weapons.Add(currentWeapon);
            SetWeaponTransform(true);
            currentWeapon.transform.rotation = Quaternion.identity;
        }
    }
}
[System.Serializable]
public class Health
{
    public float totalHealth;
    public float currentHealth;
    public OnGetAttack OnGetAttack;
    public Health(int health)
    {
        this.totalHealth = health;
        currentHealth = totalHealth;
    }
    public void GetDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
        OnGetAttack();
    }
}
public delegate void OnGetAttack();
