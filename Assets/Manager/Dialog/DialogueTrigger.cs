using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Viasual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Ink JSON Asset")]
    [SerializeField] private TextAsset inkJSON;

    private bool isPlayerInRange;
    private bool used = false;
    private PlayerControls controls;
    private void Awake()
    {
        isPlayerInRange = false;
        if (visualCue != null)
            visualCue.SetActive(false);
        controls = new PlayerControls();
    }

    // Use 2D collider callbacks to match 2D player (Rigidbody2D / Collider2D)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (used) return;
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (used) return;
            isPlayerInRange = false;
        }
    }

    private void Update()
    {
        // Only show cue if player is in range and dialogue is not currently playing
        var dm = DialogueManager.GetInstance();
        bool dialoguePlaying = dm != null && dm.dialogueIsPlaying;
        if (!used && isPlayerInRange && !dialoguePlaying)
        {
            if (visualCue != null && !visualCue.activeSelf)
                visualCue.SetActive(true);
        }
        else
        {
            if (visualCue != null && visualCue.activeSelf)
                visualCue.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (controls == null) controls = new PlayerControls();
        // Register callback for Dialog action; only run when in range
        controls.Dialog.dialogEnter.performed += OnDialogEnter;
        controls.Dialog.Enable();
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Dialog.dialogEnter.performed -= OnDialogEnter;
            controls.Dialog.Disable();
        }
    }

    private void OnDialogEnter(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (!isPlayerInRange) return;
        if (used) return;
        var dm = DialogueManager.GetInstance();
        if (dm != null)
        {
            // If a dialogue is already playing, ignore the trigger's Enter to avoid
            // re-entering/resetting the story while the player is trying to continue.
            if (!dm.dialogueIsPlaying)
            {
                dm.EnterDialogueMode(inkJSON, this);
                // If the same Enter press opened the dialogue this frame, forward it so
                // the dialogue manager can treat it as a submit (skip typing / continue).
                dm.ProcessSubmitDuringOpen();
            }
        }
    }

    public void MarkUsed()
    {
        used = true;
        if (visualCue != null) visualCue.SetActive(false);
        // Disable the collider so it can't be triggered again
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        // Optionally disable this component
        this.enabled = false;
    }


}
