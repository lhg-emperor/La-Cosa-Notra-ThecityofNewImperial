using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public IWeapon currentWeapon;

    [Header("Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private PlayerControls controls;
    private Vector2 moveInput;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public bool isAttacking = false;

    public GameObject weaponModelInstance;

    public void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        controls = new PlayerControls();
        controls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Movement.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Combat.Attack.performed += ctx => Attack();
    }

    public void Start()
    {
        EquipWeapon(GameSystemManager.Instance.GetDefaultWeapon());
    }

    public void OnEnable()
    {
        controls.Movement.Enable();
        controls.Combat.Enable();
    }

    public void OnDisable()
    {
        controls.Movement.Disable();
        controls.Combat.Disable();
    }

    public void FixedUpdate()
    {
        Move();
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

    public void Animate()
    {
        bool isMoving = moveInput != Vector2.zero;

        if (animator != null)
        {
            animator.SetBool("isRunning", isMoving);
        }

        if (spriteRenderer != null && moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
        }
    }

    public void Attack()
    {
        if (isAttacking || currentWeapon == null) return;

        isAttacking = true;

        // Trigger animation
        if (animator != null && !string.IsNullOrEmpty(currentWeapon.PlayerAttackAnimTrigger))
        {
            animator.SetTrigger(currentWeapon.PlayerAttackAnimTrigger);
        }

        // Gọi hành vi tấn công của vũ khí
        currentWeapon.PerformAttack(this);

        StartCoroutine(ResetAttack(0.5f));
    }

    public IEnumerator ResetAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
    }

    public void EquipWeapon(IWeapon newWeapon)
    {
        if (newWeapon == null)
        {
            newWeapon = GameSystemManager.Instance.GetDefaultWeapon();
        }

        // Xoá model cũ nếu có
        if (weaponModelInstance != null)
        {
            Destroy(weaponModelInstance);
        }

        // Huỷ bỏ trạng thái vũ khí cũ
        currentWeapon?.OnUnequip(this);

        currentWeapon = newWeapon;

        // Gọi logic trang bị vũ khí
        currentWeapon.OnEquip(this, transform);

        // Gắn prefab nếu có
        if (currentWeapon.WeaponModelPrefab != null)
        {
            weaponModelInstance = Instantiate(currentWeapon.WeaponModelPrefab, transform);
        }

        Debug.Log($"[Player] Đã trang bị vũ khí: {currentWeapon.WeaponName} - Loại: {currentWeapon.Type}");
    }
}
