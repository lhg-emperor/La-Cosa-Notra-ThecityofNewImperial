using UnityEngine;
using System.Collections;

public class HoliganAI : MonoBehaviour
{
    private enum State
    {
        Roaming,
        Chasing
    }

    private Holigan holigan;
    private Coroutine routine;
    private State state;

    [Header("Combat")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.0f;

    private bool isAttacking = false;

    private void Awake()
    {
        holigan = GetComponent<Holigan>();
        state = State.Roaming;
    }

    private void Start()
    {
        routine = StartCoroutine(RoamingRoutine());
    }

    private void Update()
    {
        if (state == State.Roaming && holigan.IsThreatened)
        {
            Transform target = holigan.GetTarget();
            if (target != null)
            {
                state = State.Chasing;
                if (routine != null) StopCoroutine(routine);
                routine = StartCoroutine(ChaseRoutine(target));
            }
        }
        else if (state == State.Chasing)
        {
            Transform target = holigan.GetTarget();
            if (!holigan.IsThreatened || target == null)
            {
                holigan.ClearThreat();
                if (routine != null) StopCoroutine(routine);
                holigan.Stop();
                state = State.Roaming;
                routine = StartCoroutine(RoamingRoutine());
            }
        }
    }

    private IEnumerator RoamingRoutine()
    {
        while (state == State.Roaming)
        {
            holigan.SetRunning(false);
            Vector2 target = GetRandomPosition();
            holigan.Moveto(target);

            yield return new WaitForSeconds(Random.Range(2f, 12f));
            holigan.Stop();
            yield return new WaitForSeconds(Random.Range(1f, 4f));
        }
    }

    private IEnumerator ChaseRoutine(Transform target)
    {
        holigan.SetRunning(true);
        while (state == State.Chasing)
        {
            if (target == null || !holigan.IsThreatened) break;

            holigan.Moveto(target.position);

            TryAttack(target);

            yield return new WaitForSeconds(0.2f);
        }

        holigan.Stop();
        holigan.ClearThreat();
        state = State.Roaming;
        routine = StartCoroutine(RoamingRoutine());
    }

    private void TryAttack(Transform target)
    {
        if (isAttacking) return;

        float distance = Vector2.Distance(holigan.transform.position, target.position);

        if (distance <= attackRange)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                holigan.PerformAttack(damageable);
                isAttacking = true;
                StartCoroutine(ResetAttack());
            }
        }
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private Vector2 GetRandomPosition()
    {
        float radius = Random.Range(1f, 10f);
        Vector2 dir = Random.insideUnitCircle.normalized;
        return (Vector2)transform.position + dir * radius;
    }
}
