using UnityEngine;

/// <summary>
/// Configuration for an NPC in a quest step.
/// </summary>
[System.Serializable]
public class QuestNPCConfig
{
    [Tooltip("The NPC GameObject to control (scene instance or prefab to instantiate)")]
    public GameObject npc;
    
    [Tooltip("Optional: name of NPC in scene if not assigned directly")]
    public string npcName;
    
    [Tooltip("Optional: tag to find NPC in scene")]
    public string npcTag;

    [Header("NPC Behavior")]
    [Tooltip("Type of behavior for this NPC")]
    public QuestNPCController.BehaviorType behavior = QuestNPCController.BehaviorType.None;

    [Header("MoveTo Parameters")]
    [Tooltip("Destination GameObject - will extract transform automatically")]
    public GameObject moveToDestination;
    
    [Tooltip("Optional: name of destination object in scene")]
    public string moveToDestinationName;
    
    public float pauseDistance = 6f;
    public float resumeDistance = 4f;
    public float arrivalRadius = 1.25f;
    public float checkInterval = 0.25f;

    [Header("Attack Parameters")]
    [Tooltip("If true, NPC will attack the player (target is player)")]
    public bool attackPlayer = false;
}
