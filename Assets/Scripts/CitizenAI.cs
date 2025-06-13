using System.Collections;
using UnityEngine;

public class CitizenAI : MonoBehaviour
{
    private enum State
    {
        Roaming,
        Fleeing
    }

    private Transform Player;
    public float safeDistance = 6f;
    public float feeSpeed = 10f;

    private State state;
    private Citizen citizen;
    private Coroutine routine;

    private void Awake()
    {
        citizen = GetComponent<Citizen>();
        state = State.Roaming;
    }

    private void Start()
    {
        routine = StartCoroutine(RoamingRoutine());
    }

    private IEnumerator RoamingRoutine()
    {
        while (state == State.Roaming)
        {
            citizen.SetRunning(false);
            // thời gian di chuyển
            float walkDuration = Random.Range(2f, 4f);
            Vector2 roamPosition = GetRoamingPosition();
            citizen.Moveto(roamPosition);
            yield return new WaitForSeconds(walkDuration);

            // Dừng lại một lúc
            float pauseDuration = Random.Range(1f, 4f);
            citizen.Stop(); 
            yield return new WaitForSeconds(pauseDuration);

            // Có thể quay nhìn ngó?
            if (Random.value < 0.5f)
            {
                float lookAroundTime = Random.Range(1f, 3f);
                yield return StartCoroutine(LookAround(lookAroundTime));
            }
        }
    }

    private IEnumerator LookAround(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float angle = Random.Range(-90f, 90f); 
            transform.Rotate(0, 0, angle);
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            elapsed += Random.Range(0.5f, 1.5f);
        }
    }

    private Vector2 GetRoamingPosition()
    {
        // Vị trí ngẫu nhiên 
        float radius = Random.Range(1f, 3f);
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        return (Vector2)transform.position + randomDirection * radius;
    }

    public void FleeFromPlayer(Transform playerTransform)
    {
        if(playerTransform == null) return;

        Player = playerTransform;
        state = State.Fleeing;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FleeRoutine());
    }
    private IEnumerator FleeRoutine()
    {
        citizen.SetRunning(true);
        while (state == State.Fleeing)
        {
            if (Player == null)
            {
                state = State.Roaming;
                routine = StartCoroutine(RoamingRoutine());
                yield break;
            }
            Vector2 fleeDirection = (Vector2)(transform.position - Player.position).normalized;
            Vector2 fleeTarget = (Vector2)transform.position + fleeDirection * 10f;

            citizen.Moveto(fleeTarget);

            yield return new WaitForSeconds(0.5f);

            float distance = Vector2.Distance(transform.position, Player.position);
            if(distance >= safeDistance)
            {
                citizen.Stop();
                yield return new WaitForSeconds(1f);
                citizen.SetRunning(false);
                citizen.ResetThreat(); 
                state = State.Roaming;
                routine = StartCoroutine(RoamingRoutine());
                yield break;
            }
        }
    }
    private void Update()
    {
        if (state == State.Roaming && citizen.IsThreatened)
        {
            FleeFromPlayer(GameObject.FindWithTag("Player")?.transform);
        }
    }
}
