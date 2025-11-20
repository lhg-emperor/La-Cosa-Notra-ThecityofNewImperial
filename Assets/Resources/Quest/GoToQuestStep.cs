using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToQuestStep : QuestStep
{
    [Header("Destination Config")]
    [Tooltip("Khoảng cách tối thiểu để coi là đã tới điểm")]
    [SerializeField] private float completeDistance = 0.5f;

    [Tooltip("Vị trí tuyệt đối người chơi phải tới")]
    [SerializeField] private Vector3 targetPosition;

    protected override void OnInitialize()
    {
        transform.position = targetPosition;
        Debug.Log($"QuestStep khởi tạo: Đi đến vị trí {targetPosition}");
        // Register child icon (if present) so it knows quest id / step index
        RegisterTargetIconIfPresent();
    }

   
}
