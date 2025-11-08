using UnityEngine;

public class GoToFromAtoB : QuestStep
{
    public Transform pointA;
    public Transform pointB;
    public float completeDistance = 0.5f; 

    private bool questStarted;
    private bool questCompleted;

    void OnEnable()
    {
        if (pointA != null && pointB != null)
        {
            questStarted = true;
            questCompleted = false;

            // Không tự di chuyển QuestStep, chỉ log thôi
            Debug.Log("Quest started: Đi từ A đến B");
        }
    }

    protected override void CheckStep()
    {
        if (!questStarted || questCompleted) return;

        // Kiểm tra khoảng cách từ Player đến pointB
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector2.Distance(player.transform.position, pointB.position);
        if (distance <= completeDistance)
        {
            questCompleted = true;
            questStarted = false;
            Debug.Log("Quest completed: Player đã đến B!");
            CompleteStep(); // thông báo QuestStep hoàn thành
        }
    }
}
