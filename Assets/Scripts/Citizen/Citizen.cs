using UnityEngine;

public class Citizen : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeed;
    private Animator animator;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 targetPosition;
    private bool isMoving = false;

    public LayerMask obstacleMask;

    public float Health = 105f;
    public bool IsRunning { get; private set; } = false;
    public bool IsThreatened { get; private set; } = false;

    private SpriteRenderer spriteRenderer;

    public void SetRunning(bool running)
    {
        IsRunning = running;
    }

    public void SetObstacleMask(LayerMask mask)
    {
        obstacleMask = mask;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {

        if (isMoving)
        {
            Vector2 direction = (targetPosition - rb.position);
            if (direction.magnitude < 0.1f)
            {
                Stop();
            }
            else
            {
                moveDirection = direction.normalized;
                float speed = IsRunning ? runSpeed : moveSpeed;
                rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
            }

            animator?.SetBool("isMoving", isMoving);
            animator?.SetBool("isRunning", IsRunning);

        }

        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            rb.rotation = angle - 90f;
        }

        //Kiểm tra tầm nhìn
        Vector2 rayDir = moveDirection.sqrMagnitude < 0.01f ? transform.up : moveDirection;
        float rayLength = 2.5f;
        Vector2 rayOrigin = rb.position + rayDir.normalized * 0.5f;

        int visionMask = ~0; 

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDir, rayLength, visionMask);
        Debug.DrawRay(rayOrigin, rayDir * rayLength, Color.red); // Vẽ tia kiểm tra hướng

        if (hit.collider != null && hit.collider.gameObject != gameObject)
        {
            GameObject seenObj = hit.collider.gameObject;
            string layerName = LayerMask.LayerToName(seenObj.layer);

            Debug.Log($"👁️ NPC nhìn thấy: {seenObj.name} | Tag: {seenObj.tag} | Layer: {layerName}");

            if (seenObj.CompareTag("Player"))
            {
                Debug.Log("🚨 NPC phát hiện Player qua Raycast!");
                MarkThreatened(); // hoặc phản ứng khác
            }
        }
        else
        {
            Debug.Log("👁️ Không thấy gì cả");
        }

    }

    public void Moveto(Vector2 newTarget)
    {
        targetPosition = newTarget;
        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
        moveDirection = Vector2.zero;
    }

    public void TakeDamage(float damage, Transform attacker)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            IsThreatened = true;
        }
    }

    public void MarkThreatened()
    {
        IsThreatened = true;
    }

    public void ResetThreat()
    {
        IsThreatened = false;
    }
}
