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
            if (target == null || !holigan.IsThreatened)
                break;

            holigan.Moveto(target.position);
            yield return new WaitForSeconds(0.2f);
        }

        holigan.Stop();
        holigan.ClearThreat();
        state = State.Roaming;
        routine = StartCoroutine(RoamingRoutine());
    }

    private Vector2 GetRandomPosition()
    {
        float radius = Random.Range(1f, 10f);
        Vector2 dir = Random.insideUnitCircle.normalized;
        return (Vector2)transform.position + dir * radius;
    }
}
