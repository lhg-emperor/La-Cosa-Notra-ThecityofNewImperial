using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
   [Header("DialogUI")]
   [SerializeField] private GameObject dialogPanel;
   [SerializeField] private TMPro.TextMeshProUGUI dialogText;
   private static DialogueManager instance;

   private Story currentStory;
    [Header("Options")]
    [Tooltip("If true, DialogueManager will persist across scene loads. Turn on only if you need a single manager that survives scenes.")]
    [SerializeField] private bool persistBetweenScenes = false;

    public bool isDialoguePlaying{ get; private set;}
    private PlayerControls controls;
    // Registered set of dialogues that are marked cutscene-only (StoryEvent registers these at Start)
    private HashSet<TextAsset> cutsceneOnlyDialogues = new HashSet<TextAsset>();
    

   private void Awake()
   {
       if (instance != null && instance != this)
       {
           Destroy(gameObject);
           return;
       }

        instance = this;
        if (persistBetweenScenes)
        {
            DontDestroyOnLoad(gameObject);
        }

       // Initialize input actions for dialog
       controls = new PlayerControls();
       try { controls.Dialog.Enable(); } catch { }
   }

   public static DialogueManager GetInstance
   {
       get
       {
           if (instance == null)
           {
               instance = UnityEngine.Object.FindAnyObjectByType<DialogueManager>();
               if (instance == null)
               {
                   GameObject obj = new GameObject("DialogueManager");
                    instance = obj.AddComponent<DialogueManager>();
               }
           }

           return instance;
       }
   }

    private void OnDestroy()
    {
        if (controls != null)
        {
            try { controls.Dialog.Disable(); } catch { }
            try { controls.Dispose(); } catch { }
            controls = null;
        }

        if (instance == this) instance = null;
    }
   private void Start()
    {
         dialogPanel.SetActive(false);
         isDialoguePlaying = false;
    }

    // Expose whether a dialogue is currently playing
    public bool IsDialoguePlaying()
    {
        return isDialoguePlaying;
    }

    // Expose a helper so other components can check the dialog input action
    public bool IsDialogEnterTriggered()
    {
        if (controls == null) return false;
        try { return controls.Dialog.dialogEnter.triggered; } catch { return false; }
    }

    // Allow StoryEvent to register dialogues that should only play inside cutscenes
    public void RegisterCutsceneOnly(TextAsset asset)
    {
        if (asset == null) return;
        cutsceneOnlyDialogues.Add(asset);
    }

    public void UnregisterCutsceneOnly(TextAsset asset)
    {
        if (asset == null) return;
        cutsceneOnlyDialogues.Remove(asset);
    }

    public bool IsCutsceneOnly(TextAsset asset)
    {
        if (asset == null) return false;
        return cutsceneOnlyDialogues.Contains(asset);
    }

    private void Update()
    {
        if(!isDialoguePlaying)
        {
            return;
        }
        // handle input to next dialogue line
        bool submit = false;

        // Primary: Input System action
        if (controls != null)
        {
            try { submit = controls.Dialog.dialogEnter.triggered; } catch { submit = false; }
        }

        // Fallback: new Input System keyboard (if available)
        try
        {
            var k = UnityEngine.InputSystem.Keyboard.current;
            if (k != null && !submit)
            {
                submit = k.enterKey.wasPressedThisFrame || k.numpadEnterKey.wasPressedThisFrame;
            }
        }
        catch { }

        // Last resort: legacy Input
        if (!submit)
        {
            submit = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
        }

        // If there are choices, don't advance the story here (choice handling will be implemented separately)
        if (currentStory != null && currentStory.currentChoices != null && currentStory.currentChoices.Count > 0)
        {
            // TODO: Display choices and allow selection via keyboard/gamepad
            return;
        }

        if (submit)
        {
            ContinueStory();
        }
    }
    // EnterDialogueMode: if allowCutsceneOnly==false and the asset is registered as cutscene-only,
    // the call will be ignored. StoryEvent or cutscene triggers should call with allowCutsceneOnly=true.
    public void EnterDialogueMode(TextAsset inkJSON, bool allowCutsceneOnly = false)
    {
        if (inkJSON == null) return;
        if (!allowCutsceneOnly && IsCutsceneOnly(inkJSON))
        {
            // do not start cutscene-only dialogues from free triggers
            return;
        }

        currentStory = new Story(inkJSON.text);
        isDialoguePlaying = true;
        dialogPanel.SetActive(true);

        // No timeline pausing here; dialogues do not pause PlayableDirectors in this revert.
        ContinueStory();
    }

    // Backwards-compatible overload used by older callers
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        EnterDialogueMode(inkJSON, false);
    }

    public void ExitDialogueMode()
    {
        isDialoguePlaying = false;
        dialogPanel.SetActive(false);
        currentStory = null;
        dialogText.text = "";

    }

    // Public helper to advance the current story by one line (used by Timeline/Playables)
    public void AdvanceStory()
    {
        ContinueStory();
    }

    private void ContinueStory()
    {
        if (currentStory == null)
        {
            return;
        }

        if (currentStory.canContinue)
        {
            string line = currentStory.Continue();
            dialogText.text = line;
        }
        else
        {
            ExitDialogueMode();
        }
    }
}
