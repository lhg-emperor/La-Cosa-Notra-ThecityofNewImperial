using System.Collections;
using UnityEngine;

/// <summary>
/// Quest step that auto-plays an Ink dialogue and completes when the dialogue ends.
/// Require: assign an Ink JSON (TextAsset) on the step prefab.
/// </summary>
public class TalkingQuestStep : QuestStep
{
    [Header("Dialogue")]
    [Tooltip("Ink JSON asset to play for this step")]
    [SerializeField] private TextAsset inkJSON;

    protected override void OnInitialize()
    {
        // Ensure a dialogue asset is assigned to avoid blocking the quest.
        if (inkJSON == null)
        {
            Debug.LogWarning($"TalkingQuestStep: inkJSON is not assigned on {gameObject.name}. Finishing step to avoid blocking quest.");
            FinishQuestStep();
            return;
        }

        var dm = DialogueManager.GetInstance();
        if (dm == null)
        {
            Debug.LogWarning("TalkingQuestStep: DialogueManager not found in scene. Finishing step.");
            FinishQuestStep();
            return;
        }

        // Start the dialogue and wait for it to end.
        dm.EnterDialogueMode(inkJSON, null);
        dm.ProcessSubmitDuringOpen(); // allow immediate advance if player pressed interact
        StartCoroutine(WaitForDialogue(dm));
    }

    private IEnumerator WaitForDialogue(DialogueManager dm)
    {
        // Wait until dialogue finishes
        while (dm != null && dm.dialogueIsPlaying)
        {
            yield return null;
        }

        // Dialogue ended → finish step
        FinishQuestStep();
    }
}
