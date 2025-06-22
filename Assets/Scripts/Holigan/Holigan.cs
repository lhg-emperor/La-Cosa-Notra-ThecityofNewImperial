using UnityEngine;

public class Holigan : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float health = 100f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 targetPosition;
    private bool isMoving = false;

    public bool IsRunning { get; private set; } = false;
    public bool IsThreatened { get; private set; } = false;
    private Transform currentTarget;

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

    public void Moveto(Vector2 pos)
    {
        targetPosition = pos;
        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
        moveDirection = Vector2.zero;
    }

    public void SetRunning(bool running)
    {
        IsRunning = running;
    }

    public void TakeDamage(float damage, Transform attacker)
    {
        health -= damage;
        Debug.Log($"[Holigan] HP: {health}");

        if (health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            IsThreatened = true;
            currentTarget = attacker;
        }
    }

    public Transform GetTarget()
    {
        return currentTarget;
    }

    public void ClearThreat()
    {
        IsThreatened = false;
        currentTarget = null;
    }
}
