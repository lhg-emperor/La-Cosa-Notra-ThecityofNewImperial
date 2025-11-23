// DialogueManager removed per user request. This is a minimal stub to avoid compile errors
// in other parts of the project that may reference DialogueManager. All behavior is no-op.
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;
    public static DialogueManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                instance = UnityEngine.Object.FindAnyObjectByType<DialogueManager>();
            }
            return instance;
        }
    }

    public bool IsDialoguePlaying() { return false; }
    public bool IsDialogEnterTriggered() { return false; }
    public void EnterDialogueMode(TextAsset inkJSON, bool allowCutsceneOnly = false) { }
    public void AdvanceStory() { }
    public void ExitDialogueMode() { }
    public void RegisterCutsceneOnly(TextAsset asset) { }
    public void UnregisterCutsceneOnly(TextAsset asset) { }
    public bool IsCutsceneOnly(TextAsset asset) { return false; }
}
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
