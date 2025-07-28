using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class CitizenSpawn : MonoBehaviour
{
    public List<GameObject> citizenPrefabs;
    public Transform player;
    public float SpawnRadius;
    public int targetCount;
    public float checkInterval;
    private Camera mainCamera;

    private List<GameObject> spawnCitizen = new List<GameObject>();

    private void Start()
    {
        InvokeRepeating(nameof(CheckAndSpawnCitizens), 0f, checkInterval);
        mainCamera = Camera.main;
    }

    private void CheckAndSpawnCitizens()
    {
        CleanUpCitizens();

        if (citizenPrefabs == null || citizenPrefabs.Count == 0)
        {
            Debug.LogError("citizenPrefabs bị trống hoặc chưa gán trong Inspector!");
            return;
        }

        int attempts = 0;
        while (spawnCitizen.Count < targetCount && attempts < 20)
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
                spawnCitizen.Add(newCitizen);
            }
            attempts++;
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(2, SpawnRadius);
        Vector3 randomPos = player.position + (Vector3)offset;

        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
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
