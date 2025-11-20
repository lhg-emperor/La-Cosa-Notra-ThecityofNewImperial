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
        // Register icon child (if any) so it knows quest id/step before subclasses run
        RegisterTargetIconIfPresent();
        OnInitialize();
    }


    protected virtual void OnInitialize()
    {
        // Override n?u c?n
    }

    
    // Called by the icon when the player reaches the step target.
    public void TargetReachedByPlayer()
    {
        OnTargetReached();
        FinishQuestStep();
    }

    // Subclasses can override to perform step-specific actions when the player reaches the target
    protected virtual void OnTargetReached() { }

    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;
            Debug.Log($"? QuestStep ho�n th�nh: QuestID={questId}, StepIndex={stepIndex}");
            // Th�ng b�o cho QuestManager ?? chuy?n b??c ti?p theo
            QuestManager.Instance.AdvanceQuestStep(questId);
            Destroy(this.gameObject);
        }
    }

    // When a QuestStep is instantiated, register any child QuestTargetIcon so it knows which quest/step it belongs to.
    public void RegisterTargetIconIfPresent()
    {
        var icon = GetComponentInChildren<QuestTargetIcon>(true);
        if (icon != null)
        {
            icon.SetQuestAndStep(questId, stepIndex, this);
        }
        else
        {
            Debug.LogWarning($"QuestStep ({gameObject.name}) did not find a child QuestTargetIcon. Place an icon under the step prefab to allow arrival detection.");
        }
    }
}
