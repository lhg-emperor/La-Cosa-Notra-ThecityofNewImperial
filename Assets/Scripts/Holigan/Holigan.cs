using UnityEngine;

public class Holigan : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float health = 100f;

    [Header("Combat")]
    [SerializeField] private float baseDamage = 10f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 targetPosition;
    private bool isMoving = false;

    public bool IsRunning { get; private set; } = false;
    public bool IsThreatened { get; private set; } = false;
    private Transform currentTarget;

    private bool isAttacking = false;

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

    public void PerformAttack(IDamageable target)
    {
        if (target == null) return;
        target.TakeDamage(baseDamage, this.transform);
        Debug.Log($"[Holigan] Gây damage cho mục tiêu! Damage: {baseDamage}");
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
