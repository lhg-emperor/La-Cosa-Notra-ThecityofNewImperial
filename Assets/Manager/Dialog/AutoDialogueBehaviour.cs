using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// PlayableBehaviour that starts an Ink dialogue via DialogueManager when the clip plays.
public class AutoDialogueBehaviour : PlayableBehaviour
{
    public TextAsset dialogueAsset;
    public bool allowCutsceneOnly = true;
    public bool autoAdvance = false;
    public float autoAdvanceDelay = 2f; // seconds between auto-advance presses

    // runtime
    private float timer = 0f;
    private bool wasPlaying = false;
    private bool pausedDirector = false;
    private PlayableDirector directorRef = null;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        var dm = DialogueManager.GetInstance;
        if (dialogueAsset != null && dm != null)
        {
            dm.EnterDialogueMode(dialogueAsset, allowCutsceneOnly);
        }

        // attempt to pause the director so Timeline waits for the dialogue to finish
        var resolver = playable.GetGraph().GetResolver();
        directorRef = resolver as PlayableDirector;
        if (directorRef != null)
        {
            // Pause the director; we'll resume later when dialogue ends
            try { directorRef.Pause(); pausedDirector = true; } catch { pausedDirector = false; }
        }

        timer = 0f;
        wasPlaying = dm != null && dm.IsDialoguePlaying();
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        var dm = DialogueManager.GetInstance;
        if (dm == null) return;

        bool currentlyPlaying = dm.IsDialoguePlaying();

        // Auto-advance logic
        if (autoAdvance && currentlyPlaying)
        {
            timer += (float)info.deltaTime;
            if (timer >= Mathf.Max(0.01f, autoAdvanceDelay))
            {
                // Advance one line
                dm.AdvanceStory();
                timer = 0f;
            }
        }

        // If dialogue just finished, resume director if we paused it
        if (wasPlaying && !currentlyPlaying)
        {
            if (directorRef != null && pausedDirector)
            {
                try { directorRef.Play(); } catch { }
                pausedDirector = false;
            }
        }

        wasPlaying = currentlyPlaying;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        // If dialogue still playing and we paused the director, resume to avoid leaving the timeline paused
        if (directorRef != null && pausedDirector)
        {
            try { directorRef.Play(); } catch { }
            pausedDirector = false;
        }
    }
}
