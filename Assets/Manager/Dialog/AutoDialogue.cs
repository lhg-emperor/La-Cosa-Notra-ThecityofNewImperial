using UnityEngine;

// Attach this to an NPC or cutscene GameObject that should play a dialogue automatically
// when it becomes Active (OnEnable). The AutoDialogueSequencer will manage playing
// activated dialogues in a configured order.
public class AutoDialogue : MonoBehaviour
{
    [Tooltip("Tệp Ink (TextAsset/JSON đã biên dịch) sẽ phát khi đối tượng này được kích hoạt.")]
    public TextAsset dialogueAsset;

    [Tooltip("Nếu đúng, hội thoại này chỉ dùng cho cắt cảnh.")]
    public bool cutsceneOnly = true;

    [Tooltip("Nếu đúng, AutoDialogue chỉ chạy một lần (lần đầu). Nếu sai, có thể kích hoạt lại mỗi khi đối tượng được enable.")]
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
