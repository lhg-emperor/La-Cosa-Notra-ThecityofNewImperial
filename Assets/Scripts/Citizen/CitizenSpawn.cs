using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class CitizenSpawn : MonoBehaviour
{
    [Header("Cài đặt cơ bản")]
    public List<GameObject> citizenPrefabs;
    public Transform player;
    public float SpawnRadius = 20f;
    public int targetCount = 10;
    public float checkInterval = 2f;

    [Header("Cài đặt NavMesh")]
    public float maxSampleDistance = 2f; // bán kính kiểm tra NavMesh quanh điểm random

    private Camera mainCamera;
    private readonly List<GameObject> spawnCitizen = new List<GameObject>();

    private void Start()
    {
        mainCamera = Camera.main;
        InvokeRepeating(nameof(CheckAndSpawnCitizens), 0f, checkInterval);
    }

    private void CheckAndSpawnCitizens()
    {
        CleanUpCitizens();

        if (citizenPrefabs == null || citizenPrefabs.Count == 0)
        {
            Debug.LogError("⚠ citizenPrefabs bị trống hoặc chưa gán trong Inspector!");
            return;
        }

        int attempts = 0;
        while (spawnCitizen.Count < targetCount && attempts < 50) // tăng số attempt để tìm được vùng có NavMesh
        {
            Vector3 spawnPos = GetValidSpawnPosition();
            if (spawnPos == Vector3.zero)
            {
                attempts++;
                continue;
            }

            if (!IsInCameraView(spawnPos))
            {
                int index = Random.Range(0, citizenPrefabs.Count);
                GameObject newCitizen = Instantiate(citizenPrefabs[index], spawnPos, Quaternion.identity);

                // Nếu có NavMeshAgent thì đặt vị trí thật chính xác
                NavMeshAgent agent = newCitizen.GetComponent<NavMeshAgent>();
                if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh == false)
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(spawnPos, out hit, maxSampleDistance, NavMesh.AllAreas))
                    {
                        agent.Warp(hit.position); // snap agent vào đúng NavMesh
                    }
                }

                spawnCitizen.Add(newCitizen);
            }
            attempts++;
        }
    }

    /// <summary>
    /// Random một vị trí nằm trong NavMesh quanh người chơi
    /// </summary>
    private Vector3 GetValidSpawnPosition()
    {
        Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(2f, SpawnRadius);
        Vector3 randomPos = player.position + (Vector3)offset;

        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, maxSampleDistance, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero;
    }

    private bool IsInCameraView(Vector2 position)
    {
        Vector3 viewPortPos = mainCamera.WorldToViewportPoint(position);
        return viewPortPos.x > 0 && viewPortPos.x < 1 && viewPortPos.y > 0 && viewPortPos.y < 1;
    }

    private void CleanUpCitizens()
    {
        for (int i = spawnCitizen.Count - 1; i >= 0; i--)
        {
            GameObject citizen = spawnCitizen[i];
            if (citizen == null)
            {
                spawnCitizen.RemoveAt(i);
                continue;
            }

            float distance = Vector2.Distance(player.position, citizen.transform.position);
            if (distance > SpawnRadius)
            {
                Destroy(citizen);
                spawnCitizen.RemoveAt(i);
            }
        }
    }
}
