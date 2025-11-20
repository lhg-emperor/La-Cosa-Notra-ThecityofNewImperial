using UnityEngine;
using UnityEngine.Timeline;

[TrackColor(0.2f, 0.6f, 0.9f)]
[TrackClipType(typeof(AutoDialogueClip))]
[TrackBindingType(typeof(GameObject))]
public class AutoDialogueTrack : TrackAsset
{
    // Using default mixer is fine; clips create their own PlayableBehaviour instances
}
