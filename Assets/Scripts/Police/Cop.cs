using UnityEngine;
using System.Collections;

public class Cop : MonoBehaviour, IDamageable
{
    [SerializeField] public float patrolSpeed;
    [SerializeField] public float runSpeed;

    public float CopHealth =150;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 targetPosition;

    private bool isMoving = false;
    public bool isRunning { get; private set; } = false;

    public Transform Aggressor { get; private set; } = null; // ghi nhớ thằng oánh công an
    private Coroutine logDistanceRoutine;


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
                float speed = isRunning ? runSpeed : patrolSpeed;
                rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
            }
        }
    }
    public void MoveTo(Vector2 newTarget)
    {
        targetPosition = newTarget;
        isMoving = true;
    }
    public void Stop()
    {
        isMoving = false;
        moveDirection = Vector2.zero;
    }
    public void SetRunning(bool running)
    {
        isRunning = running;
    }
    public void TakeDamage(float damage, Transform attacker)
    {
        CopHealth -= damage;
        Debug.Log($"Cop HP: {CopHealth}");
        if (attacker != null && Aggressor != attacker)
        {
            SetAggressor(attacker);
        }
        if (CopHealth <= 0)
        {
            Destroy(gameObject);
        }

    }
    public void SetAggressor(Transform aggressor)
    {
        Aggressor = aggressor;
    }
    public void ClearAgressor()
    {
        Aggressor = null;
    }
  
}
