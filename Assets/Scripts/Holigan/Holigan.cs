// Holigan.cs
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Holigan : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float health = 100f;

    [Header("Combat")]
    [SerializeField] private float baseDamage = 10f;
    public float BaseDamage => baseDamage;

    private NavMeshAgent agent;
    private Animator animator;
    private bool isMoving = false;

    public bool IsRunning { get; private set; } = false;
    public bool IsThreatened { get; private set; } = false;
    private Transform currentTarget;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        Vector3 velocity = agent.velocity;
        bool moving = velocity.sqrMagnitude > 0.1f;

        if (moving)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }

        animator?.SetBool("isMoving", moving);
    }

    public void MoveTo(Vector2 pos, bool run)
    {
        agent.speed = run ? runSpeed : moveSpeed;
        IsRunning = run;
        agent.SetDestination(pos);
        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
        agent.ResetPath();
    }

    public void SetRunning(bool running)
    {
        IsRunning = running;
        agent.speed = running ? runSpeed : moveSpeed;
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
            // Trigger threat khi bị tấn công
            IsThreatened = true;
            currentTarget = attacker;
        }
    }

    public void TriggerAttackAnim()
    {
        animator?.SetBool("isFighting", true); // bật animation attack
    }

    public void StopAttackAnim()
    {
        animator?.SetBool("isFighting", false); // tắt animation attack
    }

    public Transform GetTarget() => currentTarget;

    public void ClearThreat()
    {
        IsThreatened = false;
        currentTarget = null;
    }
}
