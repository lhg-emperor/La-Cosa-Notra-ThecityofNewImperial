using UnityEngine;

public class Citizen : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runSpeed;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private float isTakeDamage;
    public bool IsThreatened { get; private set; } = false;

    public float Health = 105f;
    public bool IsRunning { get; private set; } = false;

    public void SetRunning(bool running)
    {
        IsRunning = running;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
                rb.MovePosition(rb.position + moveDirection * (speed * Time.fixedDeltaTime));
            }
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
    public void TakeDamage(float damage)
    {
        Health -= damage;
        Debug.Log(Health);
        if(Health <= 0)
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
