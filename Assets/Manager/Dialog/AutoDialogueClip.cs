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

    // Clip capabilities (we don't require anything special)
    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<AutoDialogueBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.dialogueAsset = dialogueAsset;
        behaviour.allowCutsceneOnly = allowCutsceneOnly;
        behaviour.autoAdvance = autoAdvance;
        behaviour.autoAdvanceDelay = autoAdvanceDelay;
        // Try to capture the PlayableDirector from the owner so the behaviour can pause/resume it
        if (owner != null)
        {
            var dir = owner.GetComponent<UnityEngine.Playables.PlayableDirector>();
            if (dir != null)
            {
                behaviour.directorRef = dir;
            }
        }

        return playable;
    }
}
