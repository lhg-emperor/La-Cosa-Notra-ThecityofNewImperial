using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private PlayerControls controls;
    private Vector2 moveInput;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool Hit = false;


    void Awake()
    {
        animator = transform.GetComponent<Animator>();
        spriteRenderer = transform.GetComponent<SpriteRenderer>();


        rb = GetComponent<Rigidbody2D>();

        controls = new PlayerControls();

        controls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Movement.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Combat.Attack.performed += ctx => OnAttack();  
    }

    void OnEnable()
    {
        controls.Movement.Enable();
        controls.Combat.Enable();
    }

    void OnDisable()
    {
        controls.Movement.Disable();
    }
    private void OnAttack()
    {
        if (Hit || animator == null) return;

        
        animator.SetBool("isHit", true);
        Hit = true;

        StartCoroutine(ResetHit(0.5f)); 
    }

    void FixedUpdate()
    {
        Move();
        Animate();
    }

    void Move()
    {
        Vector2 movement = moveInput.normalized * moveSpeed * Time.fixedDeltaTime;
        if (movement != Vector2.zero)
        {
            // Di chuyển nhân vật
            rb.MovePosition(rb.position + movement);

            // Tính góc xoay theo hướng di chuyển (tính bằng radian, đổi sang độ)
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 90f;

            // Xoay nhân vật theo hướng di chuyển
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

        // Optional: lật sprite trái/phải nếu di chuyển ngang
        if (spriteRenderer != null && moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
        }

    }
    private IEnumerator ResetHit(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("isHit", false);
        Hit = false;
    }

}
