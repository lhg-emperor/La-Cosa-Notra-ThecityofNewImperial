using UnityEngine;
using System.Collections.Generic;

public class WantedSystem : MonoBehaviour
{
    [Header(" Chỉnh sửa cấp độ Truy Nã ")]
    [Range(0, 5)]
    public int WantedLevel = 0;

    [Tooltip("Số lượng Cop được điều động để trán áp")]
    [SerializeField] public int[] CopCount = new int[6] { 2, 3, 5, 9, 15, 29 };

    [Header("Thiết lập sinh Cop")]
    public GameObject CopPre;
    public Transform player;
    public float spawnRadius;
    public float checkInterval;

    private List<GameObject> activeCops = new List<GameObject>();
    private Camera mainCamera;
    public static WantedSystem Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        mainCamera = Camera.main;
        InvokeRepeating(nameof(CheckAndSpawnCops), 0f, checkInterval);
    }
    private void CheckAndSpawnCops()
    {

        CleanUpCops();
        if (WantedLevel < 0 || WantedLevel >= CopCount.Length) return;
        
        int maxCops = CopCount[WantedLevel];
        int attemps = 0;

        while(activeCops.Count < maxCops && attemps < 20)
        {
            Vector2 spawnPos = GetRandomSpawnPos();
            if(!IsInCameraView(spawnPos))
            {
                GameObject cop = Instantiate(CopPre, spawnPos, Quaternion.identity);
                activeCops.Add(cop);

                if(cop.TryGetComponent(out Cop copComppnent))
                {
                    if (WantedLevel > 0)
                    {
                        copComppnent.SetAggressor(player);
                    }
                }
            }
            attemps++;
        }
        // Đếm số Cop đang đuổi theo Player
        int chasingCops = 0;
        foreach (GameObject copObj in activeCops)
        {
            if (copObj != null && copObj.TryGetComponent(out Cop copComponent))
            {
                string targetName = copComponent.Aggressor != null ? copComponent.Aggressor.name : "null";

                if (copComponent.Aggressor != null && copComponent.Aggressor.gameObject == player.gameObject)
                    chasingCops++;
            }
        }
    }
    private Vector2 GetRandomSpawnPos()
    {
        Vector2 offSet = Random.insideUnitCircle.normalized*Random.Range(5f, spawnRadius);
        return (Vector2)player.position + offSet;
    }

    private bool IsInCameraView(Vector2 pos)
    {
        Vector3 view = mainCamera.WorldToViewportPoint(pos);
        return view.x > 0 && view.x < 1 && view.y > 0 && view.y < 1;
    }
    private void CleanUpCops()
    {
        for(int i = activeCops.Count - 1; i >= 0; i--)
        {
            GameObject cop = activeCops[i];
            if( cop == null)
            {
                activeCops.RemoveAt(i);
                continue;
            }
            float dist = Vector2.Distance(player.position, cop.transform.position);
            if (dist > spawnRadius * 1.5f)
            {
                Destroy(cop);
                activeCops.RemoveAt(i);
            }
        }
    }
    public void SetWantedLevel(int level)
    {
        int prevLevel = WantedLevel;
        WantedLevel = Mathf.Clamp(level, 0, 5);

        if (WantedLevel == 0)
        {
            DisengageAllCops();
        }
    }

    public int GetWantedLevel()
    {
        return WantedLevel;
    }
    private void DisengageAllCops()
    {
        foreach (GameObject copObj in activeCops)
        {
            if (copObj != null)
            {
                if (copObj.TryGetComponent(out CopAI ai))
                {
                    ai.ForceStopChasing(); // dừng truy đuổi ngay lập tức
                }
                else if (copObj.TryGetComponent(out Cop cop))
                {
                    cop.ClearAgressor(); // Phòng khi AI chưa kịp kích hoạt
                }
            }
        }
    }

    //Hàm Debug 
   
}
