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

    private float BaseDamage = 10f;
    private float CurrentDamage;


    private GameObject near;
    public GameObject BatPrefab;
    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();

        controls = new PlayerControls();

        controls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Movement.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Combat.Attack.performed += ctx => OnAttack();

        controls.PickUp.pick.performed += ctx => PickUp();
        controls.PutDown.drop.performed += ctx => Drop();


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
        controls.Combat.Disable();
        controls.PickUp.Disable();
        controls.PutDown.Disable();
    }

    private void OnAttack()
    {
        if (Hit || animator == null) return;

        animator.SetBool("isHit", true);
        Hit = true;

        StartCoroutine(ResetHit(0.5f));
        Debug.Log("Damage gây ra là: " + CurrentDamage);
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
        if(near !=null)
        {
            Bat bat = near.GetComponent<Bat>();
            if( bat != null&&bat.canPickUp)
            {
                Destroy(near);
                near = null;
                CurrentDamage = bat.damage;
            }
        }
    }
    private void Drop()
    {
        Debug.Log(message: "Đã nhấn nút thả");
        if (near == null && BatPrefab != null)
        {  
            
                Vector3 dropPosition = transform.position + transform.right * 1f;
                Instantiate(BatPrefab, dropPosition, Quaternion.identity);
            CurrentDamage=BaseDamage;
            Debug.Log(message: "Damage là:" + CurrentDamage);
            
        }
    }

    private void OnTriggerEnter2D(Collider2D someBat)
    {
        if (someBat.CompareTag("Weapon"))
        {
            Bat bat = someBat.GetComponent<Bat>();
            if ( bat != null && bat.canPickUp)
            {
                near = someBat.gameObject;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D someBat)
    {
        if (someBat.CompareTag("Weapon"))
        {
            if(near == someBat.gameObject)
            {
                near = null;
            }
        }
    }
}
