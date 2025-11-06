using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> questMap;

    private void Awake()
    {
        questMap = CreateQuestMap();
        Quest quest = GetQuestById("CollectGunQuest");
        Debug.Log(quest.in4.title);
        Debug.Log(quest.state);
        Debug.Log(quest.CurrentStepExits());
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
