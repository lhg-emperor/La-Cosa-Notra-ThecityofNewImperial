// AutoDialogue removed per user request. No-op stub kept for compatibility.
using UnityEngine;

public class AutoDialogue : MonoBehaviour
{
    public TextAsset dialogueAsset;
    public bool cutsceneOnly = true;
    public bool playOnlyOnce = true;
    [HideInInspector]
    public bool hasPlayed = false;
}
