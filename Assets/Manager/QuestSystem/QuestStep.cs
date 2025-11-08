using UnityEngine;

// QuestStep cơ bản, mọi step đều có thể kế thừa
public abstract class QuestStep : MonoBehaviour
{
    protected string questId;
    protected int stepIndex;

    // Khởi tạo step
    public void InitializeStep(string questId, int stepIndex)
    {
        this.questId = questId;
        this.stepIndex = stepIndex;
        Debug.Log($"QuestStep initialized: {questId} - step {stepIndex}");
    }

    // Gọi khi step hoàn thành
    protected void CompleteStep()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.CompleteQuest(questId);
        }
        Destroy(gameObject);
    }

    // Hàm này bắt buộc các step cụ thể phải implement logic riêng
    protected abstract void CheckStep();

    void Update()
    {
        CheckStep();
    }

}
