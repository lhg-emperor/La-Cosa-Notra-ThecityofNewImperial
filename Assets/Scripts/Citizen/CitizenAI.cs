using System.Collections;
using UnityEngine;

public class CitizenAI : MonoBehaviour
{
    private enum State
    {
        Roaming,
        Fleeing
    }

    public string[] dialogueLines = {
        "Hi em! Em ăn cơm chưa?",
        "Nếu 2 người đàn ông hôn nhay thì Gay Go đấy!",
        "Ey yo what's up bruh.",
        "1 nghìn 1 cái cu đơ, 2 nghìn 2 cái cu đơ 1 nghìn",
        "Biết bố mày là ai k?"
    };

    public float safeDistance = 6f;

    private bool hasTalkedToPlayer = false;
    private Transform Player;
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

            Vector2 baseDirection = Random.insideUnitCircle.normalized;
            int stepCount = Random.Range(5, 20);

            for (int i = 0; i < stepCount; i++)
            {
                float totalMove = Random.Range(3f, 10f);
                float distanceLeft = totalMove;

                while (distanceLeft > 0.1f)
                {
                    float subStep = Mathf.Min(distanceLeft, Random.Range(1f, 5f));
                    Vector2 moveTarget = (Vector2)transform.position + baseDirection * subStep;
                    citizen.Moveto(moveTarget);

                    float walkTime = subStep / (citizen.IsRunning ? 2f : 1.5f);
                    yield return new WaitForSeconds(walkTime);

                    distanceLeft -= subStep;

                    // Rẽ nhẹ trong lúc di chuyển
                    if (Random.value < 0.6f)
                    {
                        float angle = Random.Range(-30f, 30f);
                        Quaternion rotation = Quaternion.Euler(0, 0, angle);
                        baseDirection = rotation * baseDirection;
                    }
                }

                citizen.Stop();
                yield return new WaitForSeconds(Random.Range(3f, 6f));
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
        float radius = Random.Range(1f, 3f);
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        return (Vector2)transform.position + randomDirection * radius;
    }

    public void FleeFromPlayer(Transform playerTransform)
    {
        if (playerTransform == null) return;

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

            Vector2 fleeDir = (Vector2)(transform.position - Player.position).normalized;
            citizen.Moveto((Vector2)transform.position + fleeDir * 10f);

            yield return new WaitForSeconds(0.5f);

            if (Vector2.Distance(transform.position, Player.position) >= safeDistance)
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

        if (!hasTalkedToPlayer)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("playerTrigger"))
                {
                    SayRandomLine();
                    hasTalkedToPlayer = true;
                    break;
                }
            }
        }
    }

    private void SayRandomLine()
    {
        if (dialogueLines.Length == 0) return;

        int index = Random.Range(0, dialogueLines.Length);
        Debug.Log($"[Citizen nói] {dialogueLines[index]}");
    }

    private void LateUpdate()
    {
        if (hasTalkedToPlayer)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null && Vector2.Distance(transform.position, player.transform.position) > 4f)
            {
                hasTalkedToPlayer = false;
            }
        }
    }
}
