using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sequencer that plays AutoDialogue components in a specific order when they become active.
// Usage:
// - Add this component to a manager GameObject in your scene.
// - Configure the `sequence` list in the inspector with AutoDialogue components in the desired order.
// - When an AutoDialogue becomes enabled, it notifies this sequencer. The sequencer queues it
//   and, when processing, plays dialogues in the order defined by `sequence` for activated items.
public class AutoDialogueSequencer : MonoBehaviour
{
    public static AutoDialogueSequencer Instance { get; private set; }

    [Tooltip("Danh sách theo thứ tự các AutoDialogue; sequencer sẽ phát các entry được kích hoạt theo thứ tự này.")]
    public List<AutoDialogue> sequence = new List<AutoDialogue>();

    [Tooltip("Nếu đúng, sequencer sẽ tự động xử lý các kích hoạt đang chờ khi nhận được một kích hoạt.")]
    public bool autoProcessOnActivation = true;

    // pending activations (populated by AutoDialogue.Notify)
    private HashSet<AutoDialogue> pending = new HashSet<AutoDialogue>();

    // processing flag
    private bool processing = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple AutoDialogueSequencer instances found; keeping the first.");
            enabled = false;
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // Called by AutoDialogue when it becomes enabled
    public static void NotifyActivated(AutoDialogue ad)
    {
        if (ad == null) return;
        if (Instance == null)
        {
            // No sequencer present; nothing to do
            return;
        }

        Instance.OnAutoActivated(ad);
    }

    private void OnAutoActivated(AutoDialogue ad)
    {
        // If the entry should only play once and already played, ignore
        if (ad.playOnlyOnce && ad.hasPlayed) return;

        pending.Add(ad);

        if (autoProcessOnActivation && !processing)
        {
            StartCoroutine(ProcessPendingSequence());
        }
    }

    // Public API to start processing (plays dialogues for currently pending + active items according to the sequence order)
    public void StartProcessing()
    {
        if (!processing)
        {
            StartCoroutine(ProcessPendingSequence());
        }
    }

    private IEnumerator ProcessPendingSequence()
    {
        if (processing) yield break;
        processing = true;

        var dm = DialogueManager.GetInstance;

        // Iterate the configured sequence and play any pending & active AutoDialogue in that order
        for (int i = 0; i < sequence.Count; i++)
        {
            var entry = sequence[i];
            if (entry == null) continue;

            // Only process if it was activated and is active in hierarchy
            if (!pending.Contains(entry)) continue;
            if (!entry.gameObject.activeInHierarchy) continue;

            // Guard: if dialogue asset missing, just mark as played and continue
            if (entry.dialogueAsset == null)
            {
                pending.Remove(entry);
                entry.hasPlayed = true;
                continue;
            }

            // If a DialogueManager exists, start the dialogue and wait until it finishes
            if (dm != null)
            {
                // Use allowCutsceneOnly=true so these auto-dialogues (used in cutscenes) are allowed
                dm.EnterDialogueMode(entry.dialogueAsset, true);

                // Wait until DialogueManager reports it's no longer playing
                while (dm.IsDialoguePlaying()) yield return null;
            }
            else
            {
                // No DialogueManager in scene: just wait a frame as fallback
                yield return null;
            }

            // Mark played and remove from pending
            pending.Remove(entry);
            entry.hasPlayed = true;

            // Small break to allow a frame between entries
            yield return null;
        }

        processing = false;
    }
}
