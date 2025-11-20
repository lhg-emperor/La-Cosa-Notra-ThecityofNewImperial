using UnityEngine;
using UnityEngine.Playables;

public class DialogTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Ink Json")]
    [SerializeField] private TextAsset inkJSON;

    [Header("Cutscene Settings")]
    [Tooltip("Nếu đúng, hội thoại này là bắt buộc/chỉ cho cắt cảnh và phải gắn với Timeline (PlayableDirector).")]
    [SerializeField] private bool mandatoryDialogue = false;
    [Tooltip("PlayableDirector (Timeline) điều khiển khi nào hội thoại bắt buộc này được phép phát.")]
    [SerializeField] private PlayableDirector associatedTimeline;

    private bool playerInRange = false;
    private void Awake()
    {
         playerInRange = false;
        if (visualCue != null)
        {
            visualCue.SetActive(false);
        }
    }
    private void Update()
    {
        var dm = DialogueManager.GetInstance;

        // Determine whether the player is allowed to start this dialogue right now
        bool allowedToActivate = true;
        if (mandatoryDialogue)
        {
            // mandatory dialogues only allowed when the associated timeline is playing
            allowedToActivate = (associatedTimeline != null && associatedTimeline.state == PlayState.Playing);
        }
        else
        {
            // if the asset is registered as cutscene-only, free triggers should not allow activation
            if (dm != null && dm.IsCutsceneOnly(inkJSON)) allowedToActivate = false;
        }

        // Show visual cue only when player is in range and activation is allowed and no dialogue is currently playing
        bool showCue = playerInRange && allowedToActivate && (dm == null || !dm.IsDialoguePlaying());
        if (visualCue != null) visualCue.SetActive(showCue);

        if (!showCue) return;

        // Detect submit: prefer DialogueManager's InputAction, fallback to keyboard
        bool submit = false;
        if (dm != null)
        {
            try { submit = dm.IsDialogEnterTriggered(); } catch { submit = false; }
        }

        try
        {
            var k = UnityEngine.InputSystem.Keyboard.current;
            if (k != null && !submit)
            {
                submit = k.enterKey.wasPressedThisFrame || k.numpadEnterKey.wasPressedThisFrame;
            }
        }
        catch { }

        if (!submit)
        {
            submit = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
        }

        if (!submit) return;

        // Start dialogue (we already checked allowedToActivate above)
        if (dm == null) return;
        dm.EnterDialogueMode(inkJSON);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = true;
            if (visualCue != null)
            {
                visualCue.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = false;
            if (visualCue != null)
            {
                visualCue.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        // DialogueManager owns input; nothing to dispose here
    }
}
