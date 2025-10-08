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

    private Transform lastKnownAggressor;
    private Transform playerTransform;


    private void Awake()
    {
        cop = GetComponent<Cop>();
        state = State.Patrolling;
    }
    private void Start()
    {
        routine = StartCoroutine(PatrolRoutine());
        playerTransform = WantedSystem.Instance.player;
        
    }
    private void Update()
    {
        int currentWantedLevel = WantedSystem.Instance.GetWantedLevel();

        if (state == State.Patrolling)
        {
            if (cop.Aggressor != null && currentWantedLevel > 0)
            {
                state = State.Chasing;
                if (routine != null) StopCoroutine(routine);
                routine = StartCoroutine(ChaseRoutine());
            }

            if (cop.Aggressor == null && currentWantedLevel > 0)
            {
                float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                if (distToPlayer < 15f)
                {
                    cop.SetAggressor(playerTransform);
                }
            }
        }
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
                // KHÔNG xoá Aggressor nữa!
                Debug.Log("[CopAI] Em đi xa quá, anh về trồng cá nuôi rau");
                lastKnownAggressor = cop.Aggressor;
                cop.ClearAgressor();

                state = State.Patrolling;
                routine = StartCoroutine(PatrolRoutine());
                yield break;
            }
        }

    }
    public void AlertCop(Transform attacker)
    {
        cop.SetAggressor(attacker);
    }
    public void ForceStopChasing()
    {
        if (state == State.Chasing)
        {
            Debug.Log("[CopAI] Đối tượng đã không còn bị truy nã, lập tức giải tán ei");

            if (routine != null) StopCoroutine(routine);
            cop.ClearAgressor();
            state = State.Patrolling;
            routine = StartCoroutine(PatrolRoutine());
        }
    }
}
