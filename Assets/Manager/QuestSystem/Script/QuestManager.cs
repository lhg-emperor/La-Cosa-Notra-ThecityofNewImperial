using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> questMap;

    private void Awake()
    {
        questMap = CreateQuestMap();

    }

    private void OnEnable()
    {
        GameEventsManager.instance.QuestEvents.onStartQuest += StartQuest;
        GameEventsManager.instance.QuestEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.instance.QuestEvents.onFinishQuest += FinishQuest;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.QuestEvents.onStartQuest -= StartQuest;
        GameEventsManager.instance.QuestEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.instance.QuestEvents.onFinishQuest -= FinishQuest;
    }

    private void Start()
    {
        foreach(Quest quest in questMap.Values)
        {
            GameEventsManager.instance.QuestEvents.QuestStateChange(quest);

        }
    }
    private void StartQuest(string id)
    {
        Debug.Log("Bắt đầu nhiệm vụ" + id);
    }

    private void AdvanceQuest(string id)
    {
        Debug.Log("Làm nhiệm vụ kím xìn" + id);
    }

    private void FinishQuest(string id)
    {
        Debug.Log("Xong việc, hốt tiền" + id);
    }
    private Dictionary<string, Quest> CreateQuestMap()
    {
        //Load tất cả các QuestIn4SO Scriptable Obj trong đường dẫn Assets/Manager/QuestSystem/Script folder
        QuestInforSO[] allQuest = Resources.LoadAll<QuestInforSO>("Quests");

        //Tạo QuestMap
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInforSO questIn4 in allQuest)
        {
            if (idToQuestMap.ContainsKey(questIn4.id))
            {
                Debug.LogWarning("Dublicate ID found when creating quest map:" + questIn4.id);
            }
            idToQuestMap.Add(questIn4.id, new Quest(questIn4));
        }
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if(quest == null)
        {
            Debug.LogError("ID not found in the quest map" + id);

        }
        return quest;
    }
}
