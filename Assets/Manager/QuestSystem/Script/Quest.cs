using UnityEngine;

public class Quest
{
    // Static In4
    public QuestInforSO in4;

    //State In4
    public QuestState state;

    private int currentQuestStepIndex;

    public Quest(QuestInforSO questIn4)
    {
        this.in4 = questIn4;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this.currentQuestStepIndex = 0;

    }
    public void MoveToNextStep()
    {
        currentQuestStepIndex++;
    }

    public bool CurrentStepExits()
    {
        return ( currentQuestStepIndex < in4.questStepPrefabs.Length);
    }

    public void InstantiateCurrentQuestStep(Transform parentTranform)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if(questStepPrefab != null )
        {
            Object.Instantiate<GameObject>(questStepPrefab, parentTranform);
        }
    }

    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if (CurrentStepExits() )
        {
            questStepPrefab = in4.questStepPrefabs[currentQuestStepIndex];
        }
        else
        {
            Debug.LogWarning("Tried to get quest preF, but stepIndex was out of range indicating that"
                + "there's no current step: Quest_id:" + in4.id + ", stepIndex =" + currentQuestStepIndex);
        }
        return questStepPrefab;
    }
}
