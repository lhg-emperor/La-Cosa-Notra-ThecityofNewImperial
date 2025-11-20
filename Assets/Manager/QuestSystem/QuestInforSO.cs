using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfo", menuName = "Quests/QuestInfoSO")]
public class QuestInfoSO : ScriptableObject
{
    [Header("Quest Info")]
    [SerializeField] private string id;
    [SerializeField] private string questName;

    [Header("Quest Steps")]
    [SerializeField] private GameObject[] questStepPrefabs;

    [Header("Rewards")]
    [SerializeField] private int rewardMoney;
    [SerializeField] private int rewardHealth;

    // --- Public getters ---
    public string Id => id;
    public string QuestName => questName;
    public GameObject[] QuestStepPrefabs => questStepPrefabs;
    public int RewardMoney => rewardMoney;
    public int RewardHealth => rewardHealth;
}
