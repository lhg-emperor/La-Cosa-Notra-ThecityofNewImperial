using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [Tooltip("Id của quest (phải trùng với QuestInfoSO.Id)")]
    public string questId;
    [Tooltip("Tuỳ chọn: liên kết trực tiếp tới QuestInfoSO cho điểm này (ghi đè questId nếu được gán)")]
    public QuestInfoSO questInfo;

    [Tooltip("Tuỳ chọn: prefab hiển thị cho quest (dùng tham chiếu cho designer). Sẽ KHÔNG được QuestPoint tự động instantiate để tránh trùng lặp.")]
    public GameObject questPrefab;
    [Header("Step Mapping")]
    [Tooltip("Thuộc bước nào của quest (đánh số từ 0). Dùng 0 cho bước đầu tiên.")]
    public int stepIndex = 0;

    [Header("Display")]
    [Tooltip("Icon hiển thị vị trí quest (có thể là child GameObject). Nếu để trống, không làm gì")]
    public GameObject icon;

    [Tooltip("Sau khi nhận quest, ẩn icon và vô hiệu collider để không nhận lại")]
    public bool disableAfterAccept = true;

    [Tooltip("Chỉ tự động nhận khi quest đang ở trạng thái CAN_START (mặc định true)")]
    public bool onlyIfCanStart = true;

    private CircleCollider2D col;

    private void Reset()
    {
        col = GetComponent<CircleCollider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnValidate()
    {
        // ensure collider is trigger in editor
        var c = GetComponent<CircleCollider2D>();
        if (c != null) c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        TryAcceptQuest();
    }

    private bool IsPlayer(Collider2D other)
    {
        // Default: check tag "Player". If your player uses a different tag, change it or override logic.
        return other != null && other.CompareTag("Player");
    }

    public void TryAcceptQuest()
    {
        // Global guard: if QuestManager has blocked accepts (e.g. during a cutscene), do not accept
        if (QuestManager.Instance != null && QuestManager.Instance.IsAcceptsBlocked())
        {
            // optionally log for debugging
            if (QuestManager.Instance != null && QuestManager.Instance.debugLogMapping)
                Debug.Log($"QuestPoint: accepting is currently blocked (cutscene running) - '{gameObject.name}'");
            return;
        }

        // if questInfo is assigned, prefer it
        if (questInfo != null)
        {
            questId = questInfo.Id;
        }

        if (string.IsNullOrEmpty(questId))
        {
            Debug.LogWarning($"QuestPoint on '{gameObject.name}' has empty questId and no QuestInfoSO assigned");
            return;
        }

        if (QuestManager.Instance == null)
        {
            Debug.LogError("QuestPoint: QuestManager.Instance is null");
            return;
        }

        // Optional: check quest state
        if (onlyIfCanStart)
        {
            var state = QuestManager.Instance.GetQuestState(questId);
            if (state != QuestState.CAN_START)
            {
                // Not ready yet
                return;
            }
        }

        // Find QuestInfoSO for logging
        QuestInfoSO found = null;
        foreach (var q in QuestManager.Instance.allQuests)
        {
            if (q == null) continue;
            if (q.Id == questId)
            {
                found = q;
                break;
            }
        }

        string nameForLog = found != null ? found.QuestName : "(unknown)";

        // Warn if designer attached a questPrefab or questInfo that lacks QuestStep components
        if (questPrefab != null)
        {
            bool hasStep = questPrefab.GetComponent<QuestStep>() != null || questPrefab.GetComponentInChildren<QuestStep>() != null;
            if (!hasStep)
            {
                Debug.LogWarning($"QuestPoint: assigned questPrefab '{questPrefab.name}' does not contain a QuestStep component.");
            }
        }

        if (found != null)
        {
            // validate quest prefabs contain QuestStep
            if (found.QuestStepPrefabs != null && found.QuestStepPrefabs.Length > 0)
            {
                var p = found.QuestStepPrefabs[0];
                if (p != null && p.GetComponent<QuestStep>() == null && p.GetComponentInChildren<QuestStep>() == null)
                {
                    Debug.LogWarning($"QuestPoint: QuestInfoSO '{found.name}' first step prefab '{p.name}' does not contain QuestStep component.");
                }
            }
        }

        Debug.Log($"Đã nhận nhiệm vụ: {questId} - {nameForLog}");

        // Start the quest through event bus; QuestManager will instantiate the proper step prefab
        QuestEvents.RequestStart(questId);

        if (disableAfterAccept)
        {
            // hide icon if any
            if (icon != null) icon.SetActive(false);
            // disable collider to avoid repeated accepts
            if (col != null) col.enabled = false;
        }
    }
}
