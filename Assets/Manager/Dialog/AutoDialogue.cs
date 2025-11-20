using UnityEngine;

// Attach this to an NPC or cutscene GameObject that should play a dialogue automatically
// when it becomes Active (OnEnable). The AutoDialogueSequencer will manage playing
// activated dialogues in a configured order.
public class AutoDialogue : MonoBehaviour
{
    [Tooltip("The compiled Ink JSON/TextAsset to play when this object is activated.")]
    public TextAsset dialogueAsset;

    [Tooltip("If true, this dialogue is intended for use in cutscenes only.")]
    public bool cutsceneOnly = true;

    [Tooltip("If true, this AutoDialogue will only play once (first activation). If false it can trigger repeatedly each time the object is enabled.)")]
    public bool playOnlyOnce = true;

    // runtime flag
    [HideInInspector]
    public bool hasPlayed = false;

    private void OnEnable()
    {
        // Notify the sequencer that this AutoDialogue became active.
        AutoDialogueSequencer.NotifyActivated(this);
    }
    // Helper to reset hasPlayed if the designer wants to replay a sequence
    public void ResetPlayedFlag()
    {
        hasPlayed = false;
    }
}
