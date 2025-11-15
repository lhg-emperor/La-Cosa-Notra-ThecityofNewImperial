using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class GoToQuestStep : QuestStep
{
    [Header("Destination Config")]
    [Tooltip("Khoảng cách tối thiểu để coi là đã tới điểm")]
    [SerializeField] private float completeDistance = 0.5f;

    [Tooltip("Vị trí tuyệt đối người chơi phải tới")]
    [SerializeField] private Vector3 targetPosition;

    private bool playerIsNear = false;

    private void Awake()
    {
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = completeDistance;
    }

    protected override void OnInitialize()
    {
        transform.position = targetPosition;
        Debug.Log($"QuestStep khởi tạo: Đi đến vị trí {targetPosition}");
    }

    private void Update()
    {
        if (playerIsNear) FinishQuestStep();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float dist = Vector2.Distance(player.transform.position, targetPosition);
            if (dist <= completeDistance)
            {
                FinishQuestStep();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsNear = false;
    }

   
}
