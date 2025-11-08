using UnityEngine;

[System.Serializable]
public class Quest
{
    public string questId;
    public string questName;
    public string description;
    public QuestState currentState = QuestState.Locked;

    public Quest(QuestInfoSO info)
    {
        questId = info.id;
        questName = info.questName;
        description = ""; // hoặc info.description nếu có trường mô tả
        currentState = QuestState.Locked;
    }

    public Quest(string id, string name, string desc)
    {
        questId = id;
        questName = name;
        description = desc;
        currentState = QuestState.Locked;
    }

    public void StartQuest()
    {
        if (currentState == QuestState.Locked)
        {
            currentState = QuestState.Started;
            Debug.Log("Quest: " + questName + " đã được bắt đầu.");
        }
    }

    public void SetInProgress()
    {
        if (currentState == QuestState.Started)
        {
            currentState = QuestState.InProgress;
            Debug.Log("Quest " + questName + " đang được tiến hành.");
        }
    }

    public void CompleteQuest()
    {
        if (currentState == QuestState.InProgress)
        {
            currentState = QuestState.Completed;
            Debug.Log("Quest " + questName + " đã hoàn thành!");
        }
    }
}
