using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Citizen : MonoBehaviour, IDamageable
{
    public float Health = 100f;
    public bool IsRunning { get; private set; }
    public bool IsThreatened { get; private set; }

    private NavMeshAgent agent;
    private Animator animator;

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
        agent.speed = run ? 4f : 1f;
        IsRunning = run; 
        agent.SetDestination(pos);
    }

    public void TakeDamage(float dmg, Transform attacker)
    {
        Health -= dmg;
        if (Health <= 0) Destroy(gameObject);
        else IsThreatened = true;
    }

    public void ResetThreat() => IsThreatened = false;
}
