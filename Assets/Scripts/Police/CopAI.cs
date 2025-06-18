using UnityEngine;
using System.Collections;

public class CopAI : MonoBehaviour
{
    private enum State
    {
        Patrolling,
        Chasing
    }
    private State state;
    private Cop cop;
    private Coroutine routine;
    private void Awake()
    {
        cop = GetComponent<Cop>();
        state = State.Patrolling;
    }
    private void Start()
    {
        routine = StartCoroutine(PatrolRoutine());
    }
    private IEnumerator PatrolRoutine()
    {
        while(state == State.Patrolling)
        {
            cop.SetRunning(false);
            Vector2 randomTarget = GetPatrolPoint();
            cop.MoveTo(randomTarget);
            yield return new WaitForSeconds(Random.Range(3f,10f));

            cop.Stop();
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
    private Vector2 GetPatrolPoint() 
    {
        float radius = Random.Range(2f, 5.5f);
        Vector2 dir = Random.insideUnitCircle.normalized;
        return(Vector2)transform.position*dir*radius;
    }
    private IEnumerator ChaseRoutine()
    {
        cop.SetRunning(true);
        while(state == State.Chasing)
        {
            if (cop.Aggressor == null)
            {
                state = State.Patrolling;
                routine = StartCoroutine(PatrolRoutine());
                yield break;
            }
            Vector2 chaseTarget = cop.Aggressor.position;
            cop.MoveTo(chaseTarget);

            yield return new WaitForSeconds(0.5f);

            float distance = Vector2.Distance(transform.position, chaseTarget);
            if(distance > 15)
            {
                cop.ClearAgressor();
                state = State.Patrolling;
                routine = StartCoroutine(PatrolRoutine());
                yield break;
            }
        }

    }
    private void Update()
    {
        if(state == State.Patrolling && cop.Aggressor != null)
        {
            state = State.Chasing;
            if(routine != null) StopCoroutine(routine);
            routine = StartCoroutine(ChaseRoutine());
        }
    }
    public void AlertCop(Transform attacker)
    {
        cop.SetAggressor(attacker);
    }
}
