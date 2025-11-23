// AutoDialogueBehaviour removed per user request. Minimal stub kept for compatibility with Timeline.
using UnityEngine;
using UnityEngine.Playables;

public class AutoDialogueBehaviour : PlayableBehaviour
{
    public TextAsset dialogueAsset;
    public bool allowCutsceneOnly = true;
    public bool autoAdvance = false;
    public float autoAdvanceDelay = 2f;

    // No-op behaviour: Timeline clips referencing this will not control dialogue anymore.
}
