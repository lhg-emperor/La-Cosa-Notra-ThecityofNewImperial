// DialogTrigger removed per user request. Minimal stub to avoid compile errors.
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public TextAsset inkJSON;
    public bool cutsceneOnly = false;
    public bool startOnAwake = false;

    public void TriggerDialogue()
    {
        // no-op
    }

    public void ShowVisualCue(bool show) { }
}
