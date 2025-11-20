using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpQuestStep : QuestStep
{
    [Header("Pickup Config")]
    [Tooltip("Name of the target GameObject to pick up (exact name)")]
    public string targetObjectName;

    [Tooltip("Radius to detect player pickup")]
    public float pickupRadius = 0.5f;

    private GameObject targetObj;

    protected override void OnInitialize()
    {
        if (!string.IsNullOrEmpty(targetObjectName))
        {
            targetObj = GameObject.Find(targetObjectName);
            if (targetObj != null)
            {
                transform.position = targetObj.transform.position;
                Debug.Log($"PickUpQuestStep: target found at {transform.position}");
            }
            else
            {
                Debug.LogWarning($"PickUpQuestStep: target '{targetObjectName}' not found in scene.");
            }
        }
        // Register child icon (if present) so it knows quest id / step index
        RegisterTargetIconIfPresent();
    }

    protected override void OnTargetReached()
    {
        // Simulate pickup by destroying the target object if found
        if (!string.IsNullOrEmpty(targetObjectName))
        {
            var targetObj = GameObject.Find(targetObjectName);
            if (targetObj != null)
            {
                Destroy(targetObj);
            }
        }
    }
}
