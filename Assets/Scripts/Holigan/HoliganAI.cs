// HoliganAI.cs
using System.Collections;
using UnityEngine;

public class HoliganAI : MonoBehaviour
{
    private enum State { Roaming, Chasing }

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
        Transform target = holigan.GetTarget();

        if (state == State.Roaming && target != null && holigan.IsThreatened)
        {
            // Bắt đầu truy đuổi chỉ khi có threat
            state = State.Chasing;
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(ChaseRoutine(target));
        }
        else if (state == State.Chasing)
        {
            if (target == null || !holigan.IsThreatened)
            {
                // Mất threat -> quay về roaming
                holigan.ClearThreat();
                if (routine != null) StopCoroutine(routine);
                holigan.Stop();
                holigan.SetRunning(false);
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
            Vector2 destination = GetRandomPosition();
            holigan.MoveTo(destination, false);

            float roamTime = Random.Range(2f, 8f);
            yield return new WaitForSeconds(roamTime);

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

            holigan.MoveTo(target.position, true);

            // Tấn công chỉ khi ở gần mục tiêu
            float distance = Vector2.Distance(holigan.transform.position, target.position);
            if (distance <= attackRange)
            {
                TryAttack(target);
            }

            yield return new WaitForSeconds(0.2f);
        }

        holigan.Stop();
        holigan.SetRunning(false);
        holigan.ClearThreat();
        state = State.Roaming;
        routine = StartCoroutine(RoamingRoutine());
    }

    private void TryAttack(Transform target)
    {
        if (isAttacking) return;

        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            // Chỉ gây damage khi được trigger (ở gần) và animation được bật cùng lúc
            isAttacking = true;
            holigan.TriggerAttackAnim();    // bật animation tấn công
            damageable.TakeDamage(holigan.BaseDamage, this.transform);

            StartCoroutine(ResetAttack());
        }
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        holigan.StopAttackAnim();   // tắt animation sau khi attack xong
    }

    private Vector2 GetRandomPosition()
    {
        float radius = Random.Range(1f, 10f);
        Vector2 dir = Random.insideUnitCircle.normalized;
        return (Vector2)transform.position + dir * radius;
    }
}
