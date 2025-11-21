using System.Collections.Generic;
using UnityEngine;

// FinishScene: editor/debug helper
// - Press TAB to enter a 10-second input window
// - Type "Next" (case-insensitive) within 10 seconds to mark as FINISHED
//   every quest that has at least one QuestPoint present in the current scene.
// - Quests that exist in QuestManager but have no QuestPoint in the scene will NOT be finished.
public class FinishScene : MonoBehaviour
{
    private const float INPUT_TIMEOUT = 10f;
    private bool listening = false;
    private float timeLeft = 0f;
    private string buffer = "";

    private void Update()
    {
        if (!listening)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                listening = true;
                timeLeft = INPUT_TIMEOUT;
                buffer = "";
                Debug.Log("FinishScene: input mode ON (10s). Type 'Next' to finish quests present in scene.");
            }
            return;
        }

        // listening mode
        timeLeft -= Time.deltaTime;

        // Collect typed characters this frame
        string s = Input.inputString;
        if (!string.IsNullOrEmpty(s))
        {
            foreach (char c in s)
            {
                if (c == '\b')
                {
                    if (buffer.Length > 0) buffer = buffer.Substring(0, buffer.Length - 1);
                }
                else if (c == '\n' || c == '\r')
                {
                    // ignore enter
                }
                else
                {
                    buffer += c;
                }
            }
            // small debug echo
            Debug.Log($"FinishScene: buffer='{buffer}' ({timeLeft:F1}s left)");
        }

        if (buffer.Equals("next", System.StringComparison.OrdinalIgnoreCase))
        {
            listening = false;
            TryFinishQuestsInScene();
            return;
        }

        if (timeLeft <= 0f)
        {
            listening = false;
            Debug.Log("FinishScene: input timeout — no action taken.");
        }
    }

    private void TryFinishQuestsInScene()
    {
        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("FinishScene: QuestManager.Instance not found. Cannot finish quests.");
            return;
        }

        // Find all QuestPoint instances in the currently loaded scenes (include inactive)
        var points = UnityEngine.Object.FindObjectsByType<QuestPoint>(UnityEngine.FindObjectsInactive.Include, UnityEngine.FindObjectsSortMode.None);
        var ids = new HashSet<string>();
        foreach (var p in points)
        {
            if (p == null) continue;
            string qid = p.questId;
            if (p.questInfo != null && !string.IsNullOrEmpty(p.questInfo.Id)) qid = p.questInfo.Id;
            if (string.IsNullOrEmpty(qid)) continue;
            ids.Add(qid);
        }

        if (ids.Count == 0)
        {
            Debug.Log("FinishScene: No QuestPoint instances found in scene. Nothing to finish.");
            return;
        }

        // Only finish quests that are defined in QuestManager.allQuests
        var managerQuests = new HashSet<string>();
        foreach (var q in QuestManager.Instance.allQuests)
        {
            if (q == null) continue;
            if (!string.IsNullOrEmpty(q.Id)) managerQuests.Add(q.Id);
        }

        int finished = 0;
        foreach (var id in ids)
        {
            if (!managerQuests.Contains(id))
            {
                Debug.Log($"FinishScene: skipping quest '{id}' — not present in QuestManager.allQuests.");
                continue;
            }

            // mark finished via QuestManager API
            QuestManager.Instance.FinishQuest(id);
            Debug.Log($"FinishScene: marked quest '{id}' as FINISHED (had QuestPoint in scene).");
            finished++;
        }

        Debug.Log($"FinishScene: Completed — {finished} quest(s) finished.");
    }
}
