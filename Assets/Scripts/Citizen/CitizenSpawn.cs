using UnityEngine;
using System.Collections.Generic;

public class CitizenSpawn : MonoBehaviour
{
    public GameObject CitizenPre;
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
        int attemps = 0;
        while (spawnCitizen.Count < targetCount && attemps < 20) 
        {
            Vector2 spawnPos = GetRandomPositionAroundPlayer();
            if (!IsInCameraView(spawnPos)) // chỉ spawn nếu ngoài tầm nhìn camera
            {
                GameObject newCitizen = Instantiate(CitizenPre, spawnPos, Quaternion.identity);
                spawnCitizen.Add(newCitizen);
            }
            attemps++;
        }
    }
    private bool IsInCameraView(Vector2 position)
    {
        Vector3 viewPortPos = mainCamera.WorldToViewportPoint(position);
        return viewPortPos.x > 0 && viewPortPos.x < 1 && viewPortPos.y > 0 && viewPortPos.y < 1;
    }
    private Vector2 GetRandomPositionAroundPlayer()
    {
        Vector2 RandomOffSet = Random.insideUnitCircle.normalized * Random.Range(2, SpawnRadius);
        return (Vector2)player.position+RandomOffSet;
    }
    private void CleanUpCitizens()
    {
        for (int i = spawnCitizen.Count - 1; i >= 0; i--) 
        {
            GameObject citizen = spawnCitizen[i];
            if(citizen == null )
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
