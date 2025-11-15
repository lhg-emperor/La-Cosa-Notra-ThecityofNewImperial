using System.Collections.Generic;
using UnityEngine;


public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("All QuestInfo ScriptableObjects")]
    public QuestInfoSO[] allQuests;

  
    private Dictionary<string, QuestState> questStates = new Dictionary<string, QuestState>();
    private Dictionary<string, int> currentStepIndex = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("QuestManager.Instance đã tồn tại! Xoá bản cũ nếu cần.");
            Destroy(this);
            return;
        }
        Instance = this;

        // Khởi tạo trạng thái
        foreach (var quest in allQuests)
        {
            questStates[quest.Id] = QuestState.CAN_START;
            currentStepIndex[quest.Id] = 0;
        }
    }

    public void StartQuest(string questId)
    {
        if (!questStates.ContainsKey(questId)) return;

        Debug.Log($"➡️ Bắt đầu Quest: {questId}");
        questStates[questId] = QuestState.IN_PROGRESS;

        InstantiateCurrentStep(questId);
    }

   
    public void AdvanceQuestStep(string questId)
    {
        if (!questStates.ContainsKey(questId)) return;

        currentStepIndex[questId]++;

        QuestInfoSO questInfo = GetQuestInfo(questId);
        if (currentStepIndex[questId] < questInfo.QuestStepPrefabs.Length)
        {
            InstantiateCurrentStep(questId);
        }
        else
        {
            Debug.Log($"✅ Quest hoàn thành: {questId}");
            questStates[questId] = QuestState.FINISHED;
        }
    }

   
    private void InstantiateCurrentStep(string questId)
    {
        QuestInfoSO questInfo = GetQuestInfo(questId);
        int stepIndex = currentStepIndex[questId];
        if (stepIndex >= questInfo.QuestStepPrefabs.Length) return;

        GameObject stepPrefab = questInfo.QuestStepPrefabs[stepIndex];
        GameObject stepObj = Instantiate(stepPrefab);
        QuestStep questStep = stepObj.GetComponent<QuestStep>();
        if (questStep != null)
        {
            questStep.InitializeQuestStep(questId, stepIndex);
        }
        else
        {
            Debug.LogWarning($"Prefab {stepPrefab.name} không chứa QuestStep!");
        }
    }

   
    private QuestInfoSO GetQuestInfo(string questId)
    {
        foreach (var quest in allQuests)
        {
            if (quest.Id == questId) return quest;
        }
        Debug.LogError($"QuestInfoSO không tìm thấy với id {questId}");
        return null;
    }

    
    public QuestState GetQuestState(string questId)
    {
        if (questStates.ContainsKey(questId)) return questStates[questId];
        return QuestState.CAN_START;
    }
}


public enum QuestState
{
    CAN_START,
    IN_PROGRESS,
    FINISHED
}
