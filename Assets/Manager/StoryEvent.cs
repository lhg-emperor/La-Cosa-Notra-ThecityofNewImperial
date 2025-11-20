using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StoryEntry
{
    public enum TriggerType { Manual, OnQuestFinished }

    [Tooltip("Optional id for this entry (for debug)")]
    public string entryId;

    public TriggerType triggerType = TriggerType.Manual;
    [Tooltip("Quest id to watch when triggerType is OnQuestFinished")]
    public string questIdToWatch;

    [Tooltip("If true, this entry will be enqueued when the parent StoryEvent.Trigger() is called at Start")]
    public bool playOnStart = false;

    [Tooltip("Sequence of GameObjects representing cutscenes. They will be activated/deactivated in order.")]
    public GameObject[] cutscenes = new GameObject[0];

    [Tooltip("If true, timeScale is set to 0 while each cutscene in this entry plays")]
    public bool pauseGameDuringCutscene = true;

    [Tooltip("If >0, automatically advance to next cutscene after this many real-time seconds (unscaled)")]
    public float autoAdvanceSeconds = 0f;
}

// StoryEvent: mở rộng để quản lý cắt cảnh nhỏ khi một quest hoàn thành.
// Cách dùng:
// - Gán component này lên 1 GameObject trong scene (ví dụ "StoryEvent_Quest1_Cutscene").
// - Gán `questIdToWatch` và bật `triggerOnQuestFinish` để lắng nghe khi quest đó hoàn thành.
// - Gán `cutsceneObject` (ví dụ canvas/anim) — component sẽ SetActive(true/false)
//    khi cutscene bắt đầu/kết thúc.
// - Người chơi có thể nhấn `skipKey` (mặc định Enter/Return) để bỏ qua cutscene.

public class StoryEvent : MonoBehaviour
{
    [Header("Basic")]
    [Tooltip("Sự kiện Unity có thể gán hành vi trong Inspector khi StoryEvent được kích hoạt")]
    public UnityEvent onTriggered;
    [Tooltip("Nếu true, sự kiện được kích hoạt ngay khi Start() chạy")]
    public bool triggerOnStart = false;

    [Header("Startup")]
    [Tooltip("Nếu true, StoryEvent sẽ đảm bảo tất cả GameObject cutscene tham chiếu được set inactive lúc Start để tránh cutscene hiển thị sớm.")]
    public bool ensureCutscenesInactiveAtStart = true;

    [Header("Entries")]
    [Tooltip("List of scene events this StoryEvent will manage. Each entry can trigger on quest finish or be manual; each entry can contain a sequence of cutscene GameObjects.")]
    public StoryEntry[] entries = new StoryEntry[0];

    [Header("Global settings")]
    [Tooltip("Phím để bỏ qua cutscene (mặc định Enter/Return)")]
    public KeyCode skipKey = KeyCode.Return;

    // runtime state
    private bool inCutscene = false;
    private float previousTimeScale = 1f;

    // queue of pending entries to play (ensures sequential handling)
    private Queue<StoryEntry> playQueue = new Queue<StoryEntry>();

    private void OnEnable()
    {
        QuestEvents.OnQuestStateChanged += OnQuestStateChanged_Global;
    }

    private void OnDisable()
    {
        QuestEvents.OnQuestStateChanged -= OnQuestStateChanged_Global;
    }

    private void Start()
    {
        // Optionally deactivate all referenced cutscene objects at Start so they won't show before StoryEvent plays them.
        if (ensureCutscenesInactiveAtStart)
        {
            foreach (var e in entries)
            {
                if (e.cutscenes == null) continue;
                foreach (var c in e.cutscenes) if (c != null) c.SetActive(false);
            }
        }

        if (triggerOnStart) Trigger();
    }

    // Gọi để kích hoạt event thủ công (kích hoạt tất cả entries marked triggerOnStart)
    public void Trigger()
    {
        Debug.Log($"StoryEvent triggered (GameObject={gameObject.name})");
        onTriggered?.Invoke();
        // enqueue entries that are marked playOnStart
        // enqueue in reverse order so items earlier in the list run after later items
        for (int i = entries.Length - 1; i >= 0; i--)
        {
            var e = entries[i];
            if (e.playOnStart) EnqueueEntry(e);
        }
    }

    private void OnQuestStateChanged_Global(string questId, QuestState newState)
    {
        if (string.IsNullOrEmpty(questId)) return;
        if (newState != QuestState.FINISHED) return;
        // find entries that watch this quest
        Debug.Log($"StoryEvent: received QuestStateChanged for questId={questId} newState={newState}");
        var matches = new List<StoryEntry>();
        for (int i = 0; i < entries.Length; i++)
        {
            var e = entries[i];
            if (e.triggerType == StoryEntry.TriggerType.OnQuestFinished && e.questIdToWatch == questId)
            {
                matches.Add(e);
            }
        }
        // enqueue matches in reverse order so earlier entries in the inspector run after later ones
        for (int i = matches.Count - 1; i >= 0; i--)
        {
            var e = matches[i];
            Debug.Log($"StoryEvent: matching entry '{e.entryId}' found for quest {questId}, enqueueing");
            EnqueueEntry(e);
        }
    }

    private void EnqueueEntry(StoryEntry entry)
    {
        Debug.Log($"StoryEvent: EnqueueEntry called for entry '{entry.entryId}'");
        playQueue.Enqueue(entry);
        if (!inCutscene)
        {
            Debug.Log("StoryEvent: starting ProcessQueue coroutine");
            StartCoroutine(ProcessQueue());
        }
    }

    [ContextMenu("TestTrigger")]
    private void TestTrigger()
    {
        Debug.Log("StoryEvent: TestTrigger called (manual)");
        foreach (var e in entries)
        {
            if (e.playOnStart)
            {
                EnqueueEntry(e);
            }
        }
    }

    private System.Collections.IEnumerator ProcessQueue()
    {
        while (playQueue.Count > 0)
        {
            var entry = playQueue.Dequeue();
            yield return StartCoroutine(PlayEntrySequence(entry));
        }
    }

    private System.Collections.IEnumerator PlayEntrySequence(StoryEntry entry)
    {
        if (entry == null || entry.cutscenes == null || entry.cutscenes.Length == 0) yield break;
        inCutscene = true;
        Debug.Log($"StoryEvent: Playing entry '{entry.entryId}' (GameObject={gameObject.name})");
        onTriggered?.Invoke();

        foreach (var item in entry.cutscenes)
        {
            if (item == null) continue;
            Debug.Log($"StoryEvent: activating cutscene object '{item.name}' (activeBefore={item.activeSelf})");
            // activate
            item.SetActive(true);
            Debug.Log($"StoryEvent: activated '{item.name}' (activeNow={item.activeSelf})");
            if (entry.pauseGameDuringCutscene)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }

            bool finished = false;

            // If autoDuration > 0, wait that many realtime seconds
            if (entry.autoAdvanceSeconds > 0f)
            {
                float timer = 0f;
                while (timer < entry.autoAdvanceSeconds && !finished)
                {
                    if (Input.GetKeyDown(skipKey)) finished = true;
                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }
            }
            else
            {
                // wait until skip key pressed or external EndCutscene called
                while (!finished)
                {
                    if (Input.GetKeyDown(skipKey)) finished = true;
                    yield return null;
                }
            }

            // turn off current cutscene
            item.SetActive(false);
            if (entry.pauseGameDuringCutscene)
            {
                Time.timeScale = previousTimeScale;
            }
        }

        inCutscene = false;
        yield break;
    }

    // Public API to immediately stop all playing and clear queue
    public void StopAllCutscenes()
    {
        StopAllCoroutines();
        playQueue.Clear();
        inCutscene = false;
        // deactivate all referenced cutscenes
        foreach (var e in entries)
        {
            if (e.cutscenes == null) continue;
            foreach (var c in e.cutscenes) if (c != null) c.SetActive(false);
        }
    }
}
