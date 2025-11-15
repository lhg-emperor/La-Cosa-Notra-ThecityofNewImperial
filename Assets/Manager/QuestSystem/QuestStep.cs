using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class QuestStep : MonoBehaviour
{
    protected string questId;
    protected int stepIndex;
    private bool isFinished = false;

 
    public void InitializeQuestStep(string questId, int stepIndex)
    {
        this.questId = questId;
        this.stepIndex = stepIndex;
        OnInitialize();
    }


    protected virtual void OnInitialize()
    {
        // Override n?u c?n
    }

  
    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;
            Debug.Log($"? QuestStep hoàn thành: QuestID={questId}, StepIndex={stepIndex}");
            // Thông báo cho QuestManager ?? chuy?n b??c ti?p theo
            QuestManager.Instance.AdvanceQuestStep(questId);
            Destroy(this.gameObject);
        }
    }
}
