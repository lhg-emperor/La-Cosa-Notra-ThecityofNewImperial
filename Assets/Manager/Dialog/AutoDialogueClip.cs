// AutoDialogueClip removed per user request. Minimal stub kept for Timeline compatibility.
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class AutoDialogueClip : PlayableAsset, ITimelineClipAsset
{
    public TextAsset dialogueAsset;
    public bool allowCutsceneOnly = true;
    public bool autoAdvance = false;
    public float autoAdvanceDelay = 2f;

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // Create a no-op behaviour so the clip exists but does nothing
        var playable = ScriptPlayable<AutoDialogueBehaviour>.Create(graph);
        return playable;
    }
}
