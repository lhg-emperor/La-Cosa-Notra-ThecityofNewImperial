using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// PlayableBehaviour that starts an Ink dialogue via DialogueManager when the clip plays.
using System.Collections.Generic;

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
    public PlayableDirector directorRef = null; // filled by clip when possible

    // Track how many behaviours have requested a pause for each director so multiple overlapping clips don't resume prematurely
    private static Dictionary<int, int> directorPauseCounts = new Dictionary<int, int>();

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        var dm = DialogueManager.GetInstance;
        if (dialogueAsset != null && dm != null)
        {
            dm.EnterDialogueMode(dialogueAsset, allowCutsceneOnly);
        }

        // attempt to pause the director so Timeline waits for the dialogue to finish
        if (directorRef == null)
        {
            var resolver = playable.GetGraph().GetResolver();
            directorRef = resolver as PlayableDirector;
        }

        if (directorRef != null)
        {
            int id = directorRef.GetInstanceID();
            int prev = 0;
            directorPauseCounts.TryGetValue(id, out prev);
            directorPauseCounts[id] = prev + 1;

            // Only actually pause the director when transitioning from 0 -> 1
            if (prev == 0)
            {
                try { directorRef.Pause(); pausedDirector = true; } catch { pausedDirector = false; }
            }
            else
            {
                // we requested pause, but someone else already paused it
                pausedDirector = true;
            }
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

        // If dialogue just finished, resume director if this behaviour previously requested a pause
        if (wasPlaying && !currentlyPlaying)
        {
            if (directorRef != null && pausedDirector)
            {
                int id = directorRef.GetInstanceID();
                int count = 0;
                directorPauseCounts.TryGetValue(id, out count);
                count = Mathf.Max(0, count - 1);
                if (count == 0)
                {
                    directorPauseCounts.Remove(id);
                    try { directorRef.Play(); } catch { }
                }
                else
                {
                    directorPauseCounts[id] = count;
                }

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
            int id = directorRef.GetInstanceID();
            int count = 0;
            directorPauseCounts.TryGetValue(id, out count);
            count = Mathf.Max(0, count - 1);
            if (count == 0)
            {
                directorPauseCounts.Remove(id);
                try { directorRef.Play(); } catch { }
            }
            else
            {
                directorPauseCounts[id] = count;
            }

            pausedDirector = false;
        }
    }
}
