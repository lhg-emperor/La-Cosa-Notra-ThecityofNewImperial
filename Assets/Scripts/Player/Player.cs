using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float Health = 200;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerAnimatorController animCtrl;
    private PlayerControls controls;
    private playerPickup pickup;
    private PlayerTrigger playerTrigger;

    private Vector2 moveInput;
    private Vector2 playerLookAround;
    public float lookSensitivity = 5f;

    private bool Hit = false;
    private float BaseDamage = 10f;
    public float CurrentDamage;
    private GameObject near;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animCtrl = GetComponent<PlayerAnimatorController>();
        pickup = GetComponent<playerPickup>();
        playerTrigger = GetComponentInChildren<PlayerTrigger>();

        controls = new PlayerControls();
        controls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Movement.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.LookAround.look.performed += ctx => playerLookAround = ctx.ReadValue<Vector2>();
        controls.LookAround.look.canceled += ctx => playerLookAround = Vector2.zero;
        controls.Combat.Attack.performed += ctx => OnAttack();
        controls.PickUp.pick.performed += ctx => PickUp();
        controls.PutDown.drop.performed += ctx => Drop();

        CurrentDamage = BaseDamage;
    }

    void OnEnable()
    {
        controls.Movement.Enable();
        controls.Combat.Enable();
        controls.LookAround.Enable();
        controls.PickUp.Enable();
        controls.PutDown.Enable();
    }

    void OnDisable()
    {
        controls.Movement.Disable();
        controls.Combat.Disable();
        controls.LookAround.Disable();
        controls.PickUp.Disable();
        controls.PutDown.Disable();
    }

    void FixedUpdate()
    {
        Move();
        ApplyLookRotation();
        Animate();
    }

    private void Move()
    {
        Vector2 movement = moveInput.normalized * moveSpeed * Time.fixedDeltaTime;
        if (movement != Vector2.zero)
        {
            rb.MovePosition(rb.position + movement);
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
            spriteRenderer.flipX = movement.x < 0;
        }
    }

    private void ApplyLookRotation()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(playerLookAround);
        Vector2 direction = (mousePosition - transform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = Mathf.LerpAngle(rb.rotation, angle, lookSensitivity * Time.fixedDeltaTime);
    }

    private void Animate()
    {
        bool isMoving = moveInput != Vector2.zero;
        animCtrl?.OnRunning(isMoving);
        if (spriteRenderer != null && moveInput.x != 0)
            spriteRenderer.flipX = moveInput.x < 0;
    }

    private void OnAttack()
    {
        // Lấy danh sách mục tiêu hiện tại từ PlayerTrigger
        var targets = playerTrigger?.GetTargets();

        string targetNames = targets != null && targets.Count > 0
            ? string.Join(", ", targets.Select(t => ((MonoBehaviour)t).gameObject.name))
            : "Không có mục tiêu";


        if (Hit || animCtrl == null) return;
        animCtrl.Attack();
        Hit = true;

        if (playerTrigger != null)
        {
            foreach (IDamageable target in playerTrigger.GetTargets().ToList())
            {
                target.TakeDamage(pickup.CurrentDamage, this.transform);
                Debug.Log("OnAttack Hoạt động");
            }
        }

        StartCoroutine(ResetHit(0.5f));
    }

    private IEnumerator ResetHit(float delay)
    {
        yield return new WaitForSeconds(delay);
        animCtrl?.ResetAttack();
        Hit = false;
    }

    private void PickUp() => pickup.PickUp();
    private void Drop() => pickup.Drop();

    private void OnTriggerEnter2D(Collider2D someWeapon)
    {
        if (someWeapon.CompareTag("Weapon"))
        {
            IWeapon weapon = someWeapon.GetComponent<IWeapon>();
            if (weapon != null && weapon.CanPickUp)
            {
                near = someWeapon.gameObject;
                pickup.SetNearWeapon(near);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D someWeapon)
    {
        if (someWeapon.CompareTag("Weapon") && near == someWeapon.gameObject)
        {
            near = null;
            pickup.ClearNearWeapon(someWeapon.gameObject);
        }
    }

    public void SetWeaponType(string weaponName)
    {
        animCtrl?.SetWeaponType(weaponName);
    }
}
