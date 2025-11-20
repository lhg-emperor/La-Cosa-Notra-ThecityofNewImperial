using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("All QuestInfo ScriptableObjects")]
    public QuestInfoSO[] allQuests;
    [Header("Debug")]
    [Tooltip("Nếu đúng, QuestManager sẽ in thông tin mapping (ordered quests, questPointsMap, questStates) khi Awake.")]
    public bool debugLogMapping = true;

    [Tooltip("Nếu đúng, khi không có QuestPoint cho step mong đợi, sẽ hiển thị QuestPoint có index thấp nhất có sẵn làm phương án dự phòng.")]
    public bool autoShowFirstAvailablePoint = false;

  
    private Dictionary<string, QuestState> questStates = new Dictionary<string, QuestState>();
    private Dictionary<string, int> currentStepIndex = new Dictionary<string, int>();
    // Map questId -> (stepIndex -> list of QuestPoint instances)
    private Dictionary<string, Dictionary<int, List<QuestPoint>>> questPointsMap = new Dictionary<string, Dictionary<int, List<QuestPoint>>>();
    // Ordered list of quest ids (by numeric id parsed from QuestInfoSO.Id)
    private List<QuestInfoSO> orderedQuests = new List<QuestInfoSO>();
    // When true, QuestPoints should not accept/start quests (used while cutscenes play)
    private bool acceptsBlocked = false;

    public void SetAcceptsBlocked(bool blocked)
    {
        acceptsBlocked = blocked;
        if (debugLogMapping)
            Debug.Log($"QuestManager: SetAcceptsBlocked = {blocked}");
    }

    public bool IsAcceptsBlocked()
    {
        return acceptsBlocked;
    }

    private void OnEnable()
    {
        QuestEvents.OnStartQuestRequested += StartQuest;
        QuestEvents.OnAdvanceQuestRequested += AdvanceQuestStep;
        QuestEvents.OnFinishQuestRequested += FinishQuest;
    }

    private void OnDisable()
    {
        QuestEvents.OnStartQuestRequested -= StartQuest;
        QuestEvents.OnAdvanceQuestRequested -= AdvanceQuestStep;
        QuestEvents.OnFinishQuestRequested -= FinishQuest;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("QuestManager.Instance đã tồn tại! Destroying duplicate GameObject.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Khởi tạo: thu thập QuestPoint trong scene (bao gồm inactive) và sắp xếp quests theo id số
        if (allQuests == null) allQuests = new QuestInfoSO[0];

        // Use the exact order in `allQuests` as the quest progression order (Inspector order).
        orderedQuests.Clear();
        var seenIds = new HashSet<string>();
        foreach (var q in allQuests)
        {
            if (q == null) continue;
            if (string.IsNullOrEmpty(q.Id))
            {
                Debug.LogError($"QuestManager: QuestInfoSO '{q.name}' has empty Id. Skipping.");
                continue;
            }
            if (seenIds.Contains(q.Id))
            {
                Debug.LogError($"QuestManager: Duplicate quest id '{q.Id}' found (asset '{q.name}'). Skipping duplicate.");
                continue;
            }
            seenIds.Add(q.Id);
            orderedQuests.Add(q);
        }

        // Initialize quest states in the order provided by allQuests: first CAN_START, others REQUIREMENTS_NOT_MET
        for (int i = 0; i < orderedQuests.Count; i++)
        {
            var q = orderedQuests[i];
            currentStepIndex[q.Id] = 0;
            if (i == 0)
                questStates[q.Id] = QuestState.CAN_START;
            else
                questStates[q.Id] = QuestState.REQUIREMENTS_NOT_MET;
        }

        // Find all QuestPoint in the scene (include inactive) and map them by questId
        questPointsMap.Clear();
        var points = UnityEngine.Object.FindObjectsByType<QuestPoint>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None);
        foreach (var p in points)
        {
            if (p == null) continue;
            string qid = p.questId;
            if (p.questInfo != null) qid = p.questInfo.Id;
            if (string.IsNullOrEmpty(qid)) continue;
            int idx = Mathf.Max(0, p.stepIndex);
            if (!questPointsMap.ContainsKey(qid)) questPointsMap[qid] = new Dictionary<int, List<QuestPoint>>();
            var inner = questPointsMap[qid];
            if (!inner.ContainsKey(idx)) inner[idx] = new List<QuestPoint>();
            inner[idx].Add(p);
        }

        // Apply initial visibility of quest points according to state
        foreach (var kv in questStates)
        {
            UpdateQuestPointVisibility(kv.Key, kv.Value);
        }

        if (debugLogMapping)
        {
            LogQuestMapping();
        }
        // Mark manager fully initialized (other components can wait for this)
        IsInitialized = true;
    }

    // Indicates whether QuestManager finished Awake initialization
    public bool IsInitialized { get; private set; } = false;

    private void LogQuestMapping()
    {
        Debug.Log("--- QuestManager mapping dump ---");
        Debug.Log($"Ordered quests (Inspector order) count={orderedQuests.Count}");
        for (int i = 0; i < orderedQuests.Count; i++)
        {
            var q = orderedQuests[i];
            Debug.Log($"  [{i}] id={q.Id} name={q.QuestName} asset={q.name}");
        }

        Debug.Log($"QuestPointsMap keys count={questPointsMap.Count}");
        foreach (var kv in questPointsMap)
        {
            Debug.Log($"  questIdKey='{kv.Key}' stepBuckets={kv.Value.Count}");
            foreach (var stepKv in kv.Value)
            {
                Debug.Log($"    stepIndex={stepKv.Key} count={stepKv.Value.Count}");
                foreach (var p in stepKv.Value)
                {
                    if (p != null)
                        Debug.Log($"      point name={p.gameObject.name} questIdField='{p.questId}' questInfo={(p.questInfo!=null?p.questInfo.name:"null")} stepIndex={p.stepIndex} active={p.gameObject.activeSelf}");
                }
            }
        }

        Debug.Log($"Initial questStates count={questStates.Count}");
        foreach (var kv in questStates)
        {
            int curStep = currentStepIndex.ContainsKey(kv.Key) ? currentStepIndex[kv.Key] : -1;
            Debug.Log($"  questId={kv.Key} state={kv.Value} currentStepIndex={curStep}");
        }
        Debug.Log("--- End mapping dump ---");
    }

    private void Update()
    {
        // promote quests whose prerequisites (previous quests in orderedQuests) are finished
        for (int i = 0; i < orderedQuests.Count; i++)
        {
            var q = orderedQuests[i];
            if (q == null) continue;
            if (questStates.TryGetValue(q.Id, out QuestState st) && st == QuestState.REQUIREMENTS_NOT_MET)
            {
                bool ok = true;
                for (int j = 0; j < i; j++)
                {
                    var prev = orderedQuests[j];
                    if (prev == null) continue;
                    if (!questStates.ContainsKey(prev.Id) || questStates[prev.Id] != QuestState.FINISHED)
                    {
                        ok = false; break;
                    }
                }
                if (ok)
                {
                    questStates[q.Id] = QuestState.CAN_START;
                    QuestEvents.QuestStateChange(q.Id, QuestState.CAN_START);
                    UpdateQuestPointVisibility(q.Id, QuestState.CAN_START);
                    Debug.Log($"QuestManager: Quest {q.Id} is now CAN_START");
                }
            }
        }
    }

    public void StartQuest(string questId)
    {
        if (!questStates.ContainsKey(questId)) return;

        if (questStates[questId] != QuestState.CAN_START) return;

        Debug.Log($"➡️ Bắt đầu Quest: {questId}");
        questStates[questId] = QuestState.IN_PROGRESS;
        QuestEvents.QuestStateChange(questId, QuestState.IN_PROGRESS);
        // Ensure quest points visible for in-progress quest
        UpdateQuestPointVisibility(questId, QuestState.IN_PROGRESS);
        InstantiateCurrentStep(questId);
    }

   
    public void AdvanceQuestStep(string questId)
    {
        if (!questStates.ContainsKey(questId)) return;

        currentStepIndex[questId]++;

        QuestInfoSO questInfo = GetQuestInfo(questId);
        if (currentStepIndex[questId] < questInfo.QuestStepPrefabs.Length)
        {
            InstantiateCurrentStep(questId);
            // update quest points to show points for the new step
            UpdateQuestPointVisibility(questId, QuestState.IN_PROGRESS);
        }
        else
        {
            Debug.Log($"✅ Quest hoàn thành: {questId}");
            questStates[questId] = QuestState.FINISHED;
            QuestEvents.QuestStateChange(questId, QuestState.FINISHED);
            // Remove quest points from scene when quest finished
            UpdateQuestPointVisibility(questId, QuestState.FINISHED);
        }
    }

    public void FinishQuest(string questId)
    {
        if (!questStates.ContainsKey(questId)) return;

        Debug.Log($"🏁 Quest bị đánh dấu hoàn thành: {questId}");
        questStates[questId] = QuestState.FINISHED;
        QuestEvents.QuestStateChange(questId, QuestState.FINISHED);
        UpdateQuestPointVisibility(questId, QuestState.FINISHED);
    }

   
    private void InstantiateCurrentStep(string questId)
    {
        QuestInfoSO questInfo = GetQuestInfo(questId);
        int stepIndex = currentStepIndex[questId];
        if (stepIndex >= questInfo.QuestStepPrefabs.Length) return;

        GameObject stepPrefab = questInfo.QuestStepPrefabs[stepIndex];
        GameObject stepObj = Instantiate(stepPrefab);
        QuestStep questStep = stepObj.GetComponent<QuestStep>();
        if (questStep != null)
        {
            questStep.InitializeQuestStep(questId, stepIndex);
        }
        else
        {
            Debug.LogWarning($"Prefab {stepPrefab.name} không chứa QuestStep!");
        }
    }

   
    private QuestInfoSO GetQuestInfo(string questId)
    {
        foreach (var quest in allQuests)
        {
            if (quest.Id == questId) return quest;
        }
        Debug.LogError($"QuestInfoSO không tìm thấy với id {questId}");
        return null;
    }

    
    public QuestState GetQuestState(string questId)
    {
        if (questStates.ContainsKey(questId)) return questStates[questId];
        return QuestState.CAN_START;
    }

    // Public helper to obtain the current step index of a quest
    public int GetCurrentStepIndex(string questId)
    {
        if (currentStepIndex.ContainsKey(questId)) return currentStepIndex[questId];
        return -1;
    }

    // Force-set a quest's state (used by StoryEvent sequencing). This will update internal state,
    // notify listeners and update quest points visibility.
    public void ForceSetQuestState(string questId, QuestState newState)
    {
        if (string.IsNullOrEmpty(questId)) return;
        if (!questStates.ContainsKey(questId))
        {
            // initialize default tracking for unknown questId
            currentStepIndex[questId] = 0;
            questStates[questId] = QuestState.REQUIREMENTS_NOT_MET;
        }

        questStates[questId] = newState;
        QuestEvents.QuestStateChange(questId, newState);
        UpdateQuestPointVisibility(questId, newState);
    }

    private void UpdateQuestPointVisibility(string questId, QuestState state)
    {
        if (!TryGetQuestPointsInner(questId, out var inner))
        {
            Debug.LogWarning($"QuestManager: no QuestPoint entries found for questId '{questId}' (checked raw and numeric). Existing keys: {string.Join(",", questPointsMap.Keys)}");
            return;
        }
        switch (state)
        {
            case QuestState.REQUIREMENTS_NOT_MET:
                // hide all points for this quest
                foreach (var kv in inner)
                {
                    foreach (var p in kv.Value)
                        if (p != null) p.gameObject.SetActive(false);
                }
                break;
            case QuestState.CAN_START:
                // show only step 0 points, hide others
                {
                    int desired = 0;
                    if (!inner.ContainsKey(desired) && autoShowFirstAvailablePoint && inner.Count > 0)
                    {
                        desired = inner.Keys.Min();
                    }
                    foreach (var kv in inner)
                    {
                        bool show = kv.Key == desired;
                        foreach (var p in kv.Value)
                            if (p != null) p.gameObject.SetActive(show);
                    }
                }
                break;
            case QuestState.IN_PROGRESS:
                // show only points for current step index
                {
                    int cur = 0;
                    if (currentStepIndex.ContainsKey(questId)) cur = currentStepIndex[questId];
                    int desired = cur;
                    if (!inner.ContainsKey(desired) && autoShowFirstAvailablePoint && inner.Count > 0)
                    {
                        desired = inner.Keys.Min();
                    }
                    foreach (var kv in inner)
                    {
                        bool show = kv.Key == desired;
                        foreach (var p in kv.Value)
                            if (p != null) p.gameObject.SetActive(show);
                    }
                }
                break;
            case QuestState.FINISHED:
                // destroy all points for this quest
                foreach (var kv in inner)
                {
                    foreach (var p in kv.Value)
                        if (p != null) Destroy(p.gameObject);
                }
                inner.Clear();
                break;
        }
    }

    // Attempt to retrieve the inner dictionary for a questId.
    // Accepts exact string match; if not found, tries to parse numeric value and match any key whose numeric value equals it.
    private bool TryGetQuestPointsInner(string questId, out Dictionary<int, List<QuestPoint>> inner)
    {
        if (questPointsMap.TryGetValue(questId, out inner)) return true;
        if (int.TryParse(questId, out int qNum))
        {
            foreach (var kv in questPointsMap)
            {
                if (int.TryParse(kv.Key, out int kNum) && kNum == qNum)
                {
                    inner = kv.Value;
                    return true;
                }
            }
        }
        inner = null;
        return false;
    }

    // Public helper: get quest points for a specific quest step (may be empty)
    public List<QuestPoint> GetQuestPointsForStep(string questId, int stepIndex)
    {
        if (questPointsMap.ContainsKey(questId))
        {
            var inner = questPointsMap[questId];
            if (inner.ContainsKey(stepIndex))
            {
                return new List<QuestPoint>(inner[stepIndex]);
            }
        }
        return new List<QuestPoint>();
    }
}
 
