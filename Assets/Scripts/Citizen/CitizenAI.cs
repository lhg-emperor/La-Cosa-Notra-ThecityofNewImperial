using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CitizenAI : MonoBehaviour
{
    public float safeDistance = 6f;
    public string[] dialogueLines;

    private enum State { Roaming, Fleeing }
    private State state = State.Roaming;

    private Citizen citizen;
    private Coroutine routine;
    private Transform player;
    private bool hasTalked;

    private void Awake() => citizen = GetComponent<Citizen>();

    private void Start() => routine = StartCoroutine(Roam());

    private IEnumerator Roam()
    {
        NavMeshAgent agent = citizen.GetComponent<NavMeshAgent>();

        while (true)
        {
            yield return null;
            // --- 1. Chọn điểm phía trước mặt ---
            Vector2 forward = transform.up; // NPC nhìn theo trục Y
            float angleOffset = Random.Range(-60f, 60f); // Giới hạn vùng 120 độ phía trước
            Quaternion rotation = Quaternion.Euler(0, 0, angleOffset);
            Vector2 direction = rotation * forward;
            Vector2 destination = (Vector2)transform.position + direction.normalized * Random.Range(3f, 8f);

            citizen.MoveTo(destination, false);

            float stuckTimer = 0f;
            bool isStuck = false;

            // --- 2. Theo dõi đường đi ---
            while (Vector2.Distance(transform.position, destination) > 0.5f)
            {
                if (!agent.hasPath && !agent.pathPending)
                {
                    Debug.LogWarning($"[Citizen] Không thể tìm đường tới {destination}, đứng chôn chân ở {transform.position}");
                    isStuck = true;
                    break;
                }

                stuckTimer += Time.deltaTime;
                if (stuckTimer > 25f)
                {
                    Debug.LogWarning($"[Citizen] Bị kẹt quá lâu khi đi tới {destination}");
                    isStuck = true;
                    break;
                }

                yield return null;
            }

            // --- 3. Xử lý khi bị kẹt ---
            if (isStuck)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }
                
            // --- 4. Dừng lại tạm ---
            citizen.MoveTo(transform.position, false);
            yield return new WaitForSeconds(Random.Range(0.75f, 1f));

            // --- 5. 40% nghỉ dài ---
            if (Random.value > 0.6f) // 40% nghỉ
            {
                float idleTime = Random.Range(9f, 15f);
                float elapsed = 0f;

                while (elapsed < idleTime)
                {
                    yield return new WaitForSeconds(2f);
                    elapsed += 2f;

                    if (Random.value < 0.54f)
                    {
                        float angle = Random.Range(-30f, 30f);
                        transform.Rotate(0f, 0f, angle);
                    }
                }
            }

            // --- 6. Nếu đi tiếp, sẽ chọn lại điểm trước mặt ---
        }
    }

    private IEnumerator Flee()
    {
        while (true)
        {
            if (player == null) yield break;

            Vector2 dir = ((Vector2)transform.position - (Vector2)player.position).normalized;
            citizen.MoveTo((Vector2)transform.position + dir * 10f, true);

            if (Vector2.Distance(transform.position, player.position) >= safeDistance)
            {
                citizen.ResetThreat();
                state = State.Roaming;
                routine = StartCoroutine(Roam());
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Update()
    {
        if (state == State.Roaming && citizen.IsThreatened)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            if (player != null)
            {
                state = State.Fleeing;
                if (routine != null) StopCoroutine(routine);
                routine = StartCoroutine(Flee());
            }
        }

        if (!hasTalked)
        {
            foreach (var hit in Physics2D.OverlapCircleAll(transform.position, 2f))
            {
                if (hit.CompareTag("playerTrigger"))
                {
                    SaySomething();
                    hasTalked = true;
                    break;
                }
            }
        }
    }

    private void SaySomething()
    {
        if (dialogueLines == null || dialogueLines.Length == 0) return;
        Debug.Log($"[Citizen nói] {dialogueLines[Random.Range(0, dialogueLines.Length)]}");
    }
}
