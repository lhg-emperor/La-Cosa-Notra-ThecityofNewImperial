using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestInfo", menuName = "Quest/QuestInfoSO")]
public class QuestInfoSO : ScriptableObject
{
    [Header("Quest Identity")]
    public string id;
    public string questName;

    [Header("Progress Requirement")]

    [Header("Rewards")]
    public int rewardMoney = 0;

    [Header("Quest Steps")]
    public GameObject[] questStepPrefabs; 

}
