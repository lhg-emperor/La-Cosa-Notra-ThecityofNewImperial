using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMPro.TextMeshProUGUI dialogueText;

    private Story currentStory;
    private static DialogueManager instance;
    private DialogueTrigger currentTrigger;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // Ensure this GameObject is a root object before marking DontDestroyOnLoad
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
    }

    // input handled via PlayerControls callback (OnDialogSubmit)

    public void EnterDialogueMode(TextAsset inkJSON, DialogueTrigger trigger = null)
    {
        if (inkJSON == null) return;
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        canContinueToNextLine = false;
        currentTrigger = trigger;
        ContinueStory();
    }

    // Called when dialogue is opened by an input that occurred in the same frame.
    // This allows the same Enter press that opened the dialogue to also act as a submit
    // (e.g., skip typing or immediately continue) instead of being ignored.
    public void ProcessSubmitDuringOpen()
    {
        if (!dialogueIsPlaying) return;

        if (isTyping)
        {
            // finish current line immediately
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
                displayLineCoroutine = null;
            }
            if (dialogueText != null)
                dialogueText.maxVisibleCharacters = dialogueText.text.Length;
            if (continueIcon != null) continueIcon.SetActive(true);
            canContinueToNextLine = true;
            isTyping = false;
            ignoreNextSubmitAfterOpen = true;
            return;
        }

        if (canContinueToNextLine)
        {
            ContinueStory();
            // After forwarding the submit from the opener, ignore the next submit callback
            ignoreNextSubmitAfterOpen = true;
        }
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        currentStory = null;
        if (currentTrigger != null)
        {
            currentTrigger.MarkUsed();
            currentTrigger = null;
        }
    }

    private void ContinueStory()
    {
        if (currentStory == null) { ExitDialogueMode(); return; }

        if (currentStory.canContinue)
        {
            string nextLine = currentStory.Continue();
            if (displayLineCoroutine != null) StopCoroutine(displayLineCoroutine);
            displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
        }
        else
        {
            ExitDialogueMode();
        }
    }
    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject continueIcon;

    public bool dialogueIsPlaying { get; private set; }

    private bool canContinueToNextLine = false;
    private bool isTyping = false;
    private Coroutine displayLineCoroutine;

    private PlayerControls controls;
    private bool ignoreNextSubmitAfterOpen = false;

    private void OnEnable()
    {
        if (controls == null) controls = new PlayerControls();
        controls.Dialog.dialogEnter.performed += OnDialogSubmit;
        controls.Dialog.Enable();
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Dialog.dialogEnter.performed -= OnDialogSubmit;
            controls.Dialog.Disable();
        }
    }

    private void OnDialogSubmit(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        // If we were just opened and already processed the opening submit, ignore the next submit callback
        if (ignoreNextSubmitAfterOpen)
        {
            ignoreNextSubmitAfterOpen = false;
            return;
        }
        if (!dialogueIsPlaying) return;

        if (isTyping)
        {
            // finish current line immediately
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
                displayLineCoroutine = null;
            }
            dialogueText.maxVisibleCharacters = dialogueText.text.Length;
            if (continueIcon != null) continueIcon.SetActive(true);
            canContinueToNextLine = true;
            isTyping = false;
            return;
        }

        if (canContinueToNextLine)
        {
            ContinueStory();
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        if (dialogueText == null) yield break;

        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
        if (continueIcon != null) continueIcon.SetActive(false);

        canContinueToNextLine = false;
        isTyping = true;

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.maxVisibleCharacters++;
            yield return new WaitForSeconds(typingSpeed);
        }

        // finished typing
        if (continueIcon != null) continueIcon.SetActive(true);
        canContinueToNextLine = true;
        isTyping = false;
        displayLineCoroutine = null;
    }
}
