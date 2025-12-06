using System.Collections;
using UnityEngine;

// Optional async hooks for behaviors that need to run/cue work and signal completion
public interface IQuestStepBehaviorAsync
{
    // Coroutine invoked when the player arrives at the step target.
    // QuestStep will yield on this IEnumerator until it completes.
    IEnumerator OnPlayerArrivedAsync(QuestStep step);
}
