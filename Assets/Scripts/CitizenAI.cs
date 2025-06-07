using System.Collections;
using UnityEngine;

public class CitizenAI : MonoBehaviour
{
    private enum State
    {
        Roaming
    }

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
            // Random thời gian di chuyển
            float walkDuration = Random.Range(2f, 5f);
            Vector2 roamPosition = GetRoamingPosition();
            citizen.Moveto(roamPosition);
            yield return new WaitForSeconds(walkDuration);

            // Dừng lại một lúc
            float pauseDuration = Random.Range(1f, 4f);
            citizen.Stop(); // Giả sử Citizen có hàm dừng lại
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
            float angle = Random.Range(-90f, 90f); // Quay trái/phải
            transform.Rotate(0, 0, angle);
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            elapsed += Random.Range(0.5f, 1.5f);
        }
    }

    private Vector2 GetRoamingPosition()
    {
        // Vị trí tương đối ngẫu nhiên quanh NPC
        float radius = Random.Range(1f, 3f);
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        return (Vector2)transform.position + randomDirection * radius;
    }
}
