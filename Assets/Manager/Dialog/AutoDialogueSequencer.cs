// AutoDialogueSequencer removed per user request. Minimal stub kept for compatibility.
using UnityEngine;
using System.Collections.Generic;

public class AutoDialogueSequencer : MonoBehaviour
{
    public static AutoDialogueSequencer Instance { get; private set; }
    public List<AutoDialogue> sequence = new List<AutoDialogue>();
    public bool autoProcessOnActivation = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public static void NotifyActivated(AutoDialogue ad) { }
    public void StartProcessing() { }
}
