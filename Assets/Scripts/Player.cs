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

    [Header("Weapon Pickup")]
    private GameObject near;
    private GameObject held;

    private float BaseDamage = 10f;
    private float CurrentDamage;

    void Awake()
    {
        animator = transform.GetComponent<Animator>();
        spriteRenderer = transform.GetComponent<SpriteRenderer>();


        rb = GetComponent<Rigidbody2D>();

        controls = new PlayerControls();

        controls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Movement.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Combat.Attack.performed += ctx => OnAttack();

        controls.PickUp.pick.performed += ctx => PickUp();
        controls.PutDown.drop.performed += ctx => Drop();

        //Chỉ số tấn công của thg Main-Vito
        CurrentDamage = BaseDamage;
    }

    void OnEnable()
    {
        controls.Movement.Enable();
        controls.Combat.Enable();
        controls.PickUp.Enable();
        controls.PutDown.Enable();
    }

    void OnDisable()
    {
        controls.Movement.Disable();
        controls.PickUp.Disable();
        controls.PutDown.Disable();
    }
    private void OnAttack()
    {
        if (Hit || animator == null) return;

        
        animator.SetBool("isHit", true);
        Hit = true;

        StartCoroutine(ResetHit(0.5f));
        Debug.Log(message: "Damage gây ra là:"+CurrentDamage);
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
    private void PickUp()
    {
        Debug.Log(message:"Đa kích hoạt điều kiện");
        if (held == null && near != null)
        {
            held = Instantiate(near);
            held.SetActive(false);
            Debug.Log(message:"Đã lụm");

            Bat bat = held.GetComponent<Bat>();
            if ( bat != null)
            {
                CurrentDamage = bat.batDamage;
                Debug.Log(message: "Đã trang bị Bat, damage hiện tại là" + CurrentDamage);
            }
            else
            {
                CurrentDamage = BaseDamage;
                Debug.Log("Không tìm thấy Bat script, Damage trở về mặc định.");
            }
        }
        else if(held != null) 
        {
            Debug.Log(message: "Đang cầm");
        }
        else if(near == null)
        {
            Debug.Log(message: "K ở gần để pick");
        }
    }
    private void Drop()
    {
        Debug.Log("Do m ngu, bố éo có lỗi");
        Debug.Log(held);
        if (held != null)
        {
            held.transform.position = transform.position + transform.right * 1f;
            held.SetActive(true);
            held = null;
            Debug.Log(message: "Đã thả");
        }
        else
        {
            Debug.Log(message: "Có cầm éo gì đâu mà m thả");
        }
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Weapon"))
        {
            Bat bat = other.GetComponent<Bat>();
            Debug.Log(bat);
            Debug.Log(bat.canPickUp);
            if (bat != null && bat.canPickUp)
            {
                near = bat.gameObject;
            }
            else if(bat == null)
            {
                Debug.Log(message: "K tìm thấy ");
            }
        
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Weapon"))
        {
            if (near == other.gameObject)
            {
                near = null;
            }
        }
    }
}
