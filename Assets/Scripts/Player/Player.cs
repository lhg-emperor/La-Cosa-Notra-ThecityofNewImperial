using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public WeaponManager currentWeapon;

    [Header("Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private PlayerControls controls;
    private Vector2 moveInput;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public bool isHit = false;

    private KatanaPickup nearbyKatanaPickup;


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
        controls.Pickup.Enable();

        // Đăng ký sự kiện nhấn phím Pickup (ví dụ phím R)
        controls.Pickup.Pickup.performed += ctx => TryPickupWeapon();
    }

    public void OnDisable()
    {
        controls.Movement.Disable();
        controls.Combat.Disable();
        controls.Pickup.Disable();

        // Hủy đăng ký sự kiện để tránh gọi nhiều lần
        controls.Pickup.Pickup.performed -= ctx => TryPickupWeapon();
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
        if (currentWeapon == null) return;

        if (isHit) return;

        if (animator != null)
        {
            if (currentWeapon is EmptyWeapon)
            {
                animator.SetBool("isHit", true);
                isHit = true;
                StartCoroutine(ResetHitBool(0.3f)); // 0.3s là thời gian animation hit, chỉnh theo thực tế
            }
            else
            {
                string triggerName = currentWeapon.GetAttackAnimationTrigger();

                if (!string.IsNullOrEmpty(triggerName))
                {
                    animator.SetTrigger(triggerName);
                }
            }
        }
        currentWeapon.PerformAttack(this);

    }
    public void SetNearbyWeaponPickup(KatanaPickup pickup)
    {
        nearbyKatanaPickup = pickup;
    }

    public void TryPickupWeapon()
    {
        Debug.Log($"[Player] nearbyKatanaPickup = {nearbyKatanaPickup}, tồn tại? {(nearbyKatanaPickup != null && nearbyKatanaPickup.gameObject != null)}");
        if (nearbyKatanaPickup != null && nearbyKatanaPickup.gameObject != null)
        {
            Debug.Log("[Player] Đang cố gắng nhặt Katana...");
            nearbyKatanaPickup.GiveKatana();
        }
        else
        {
            Debug.LogWarning("[Player] Không có Katana để nhặt hoặc vật thể không còn tồn tại.");
            nearbyKatanaPickup = null; // reset lại để tránh giữ ref chết
        }
    }



    private IEnumerator ResetHitBool(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("isHit", false);
        isHit = false;
    }
    private IEnumerator ResetAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        isHit = false;
    }

    public void EquipWeapon(WeaponManager newWeapon)
    {
        if (newWeapon == null)
        {
            newWeapon = GameSystemManager.Instance.GetDefaultWeapon();
        }

        currentWeapon?.OnUnequip(this);
        currentWeapon = newWeapon;
        currentWeapon.OnEquip(this);

        Debug.Log($"[Player] Đã trang bị vũ khí: {currentWeapon.WeaponName} | Loại: {currentWeapon.Type} | Sát thương: {currentWeapon.Damage} | Tầm đánh: {currentWeapon.Range}m");
    }

}
