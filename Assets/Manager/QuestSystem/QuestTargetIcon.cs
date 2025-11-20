using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class QuestTargetIcon : MonoBehaviour
{
    [Tooltip("Quest id (optional if questInfo assigned on parent step)")]
    public string questId;
    [Tooltip("Step index this icon represents")]
    public int stepIndex = 0;

    private QuestStep parentStep;

    private void Awake()
    {
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
    }

    // Called by QuestStep when the step is initialized so we know which quest/step this icon belongs to
    public void SetQuestAndStep(string qid, int idx, QuestStep step)
    {
        questId = qid;
        stepIndex = idx;
        parentStep = step;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (string.IsNullOrEmpty(questId))
        {
            Debug.LogWarning($"QuestTargetIcon on '{gameObject.name}' has empty questId.");
            return;
        }

        var state = QuestManager.Instance.GetQuestState(questId);
        int cur = QuestManager.Instance.GetCurrentStepIndex(questId);

        // Only react when this icon corresponds to the current active step for the quest
        if (cur != stepIndex)
        {
            return;
        }

        // Notify the parent QuestStep if available, otherwise request advance directly
        if (parentStep != null)
        {
            parentStep.TargetReachedByPlayer();
        }
        else
        {
            QuestEvents.RequestAdvance(questId);
        }
    }
}
