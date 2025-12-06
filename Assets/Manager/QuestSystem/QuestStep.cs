using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class QuestStep : MonoBehaviour
{
    protected string questId;
    protected int stepIndex;
    private bool isFinished = false;

    /// <summary>
    /// Serializable class to configure NPC behaviors for this step.
    /// </summary>
    [Header("Quest NPCs")]
    [Tooltip("Configure NPCs and their behaviors for this step")]
    public QuestNPCConfig[] questNPCs = new QuestNPCConfig[0];

    // Track active NPC controllers started by this step
    private List<QuestNPCController> activeControllers = new List<QuestNPCController>();
    // Track spawned NPC instances (from prefabs) for cleanup
    private List<GameObject> spawnedNpcInstances = new List<GameObject>();

    public void InitializeQuestStep(string questId, int stepIndex)
    {
        this.questId = questId;
        this.stepIndex = stepIndex;
        RegisterTargetIconIfPresent();
        OnInitialize();

        // Start NPC behaviors configured in questNPCs
        if (questNPCs != null && questNPCs.Length > 0)
        {
            foreach (var qnpc in questNPCs)
            {
                if (qnpc == null) continue;

                // Resolve NPC GameObject
                GameObject npcGo = ResolveNPC(qnpc);
                if (npcGo == null)
                {
                    Debug.LogWarning($"QuestStep: Cannot resolve NPC for behavior '{qnpc.behavior}'");
                    continue;
                }

                // Get or add QuestNPCController
                var ctrl = npcGo.GetComponent<QuestNPCController>();
                if (ctrl == null)
                {
                    Debug.LogWarning($"QuestStep: NPC '{npcGo.name}' missing QuestNPCController component");
                    continue;
                }

                // Start behavior based on type
                switch (qnpc.behavior)
                {
                    case QuestNPCController.BehaviorType.MoveTo:
                        StartMoveToNPC(ctrl, qnpc);
                        break;
                    case QuestNPCController.BehaviorType.Attack:
                        StartAttackNPC(ctrl, qnpc);
                        break;
                    case QuestNPCController.BehaviorType.None:
                    default:
                        break;
                }

                if (!activeControllers.Contains(ctrl))
                    activeControllers.Add(ctrl);
            }
        }
    }

    /// <summary>
    /// Resolve NPC GameObject from direct reference, name, tag, or instantiate if prefab.
    /// </summary>
    private GameObject ResolveNPC(QuestNPCConfig qnpc)
    {
        GameObject npcGo = qnpc.npc;

        // Try resolve by name
        if (npcGo == null && !string.IsNullOrEmpty(qnpc.npcName))
        {
            npcGo = GameObject.Find(qnpc.npcName);
        }

        // Try resolve by tag
        if (npcGo == null && !string.IsNullOrEmpty(qnpc.npcTag))
        {
            try
            {
                var found = GameObject.FindGameObjectsWithTag(qnpc.npcTag);
                if (found != null && found.Length > 0) npcGo = found[0];
            }
            catch { }
        }

        if (npcGo == null) return null;

        // If NPC is a prefab asset (not in scene), instantiate it
        if (!npcGo.scene.IsValid())
        {
            var spawned = GameObject.Instantiate(npcGo);
            spawned.name = npcGo.name + "_inst";
            spawnedNpcInstances.Add(spawned);
            npcGo = spawned;
        }

        return npcGo;
    }

    private void StartMoveToNPC(QuestNPCController ctrl, QuestNPCConfig qnpc)
    {
        // Resolve destination
        Transform dest = null;
        
        // If moveToDestination is assigned, extract its transform
        if (qnpc.moveToDestination != null)
        {
            dest = qnpc.moveToDestination.transform;
        }
        
        // Try by name if not found
        if (dest == null && !string.IsNullOrEmpty(qnpc.moveToDestinationName))
        {
            var destGo = GameObject.Find(qnpc.moveToDestinationName);
            if (destGo != null) dest = destGo.transform;
        }

        if (dest == null)
        {
            Debug.LogWarning($"QuestStep: MoveTo behavior missing destination");
            return;
        }

        ctrl.StartMoveTo(dest, qnpc.pauseDistance, qnpc.resumeDistance, qnpc.arrivalRadius, qnpc.checkInterval, null);
    }

    private void StartAttackNPC(QuestNPCController ctrl, QuestNPCConfig qnpc)
    {
        Transform playerTarget = null;
        
        if (qnpc.attackPlayer)
        {
            var playerGo = GameObject.FindWithTag("Player");
            if (playerGo != null) playerTarget = playerGo.transform;
            else
            {
                var player = FindObjectOfType<Player>();
                if (player != null) playerTarget = player.transform;
            }
        }

        if (playerTarget == null)
        {
            Debug.LogWarning($"QuestStep: Attack behavior cannot find player target");
            return;
        }

        ctrl.StartAttack(playerTarget, null);
    }

    protected virtual void OnInitialize()
    {
        // Override if needed
    }

    public void TargetReachedByPlayer()
    {
        OnTargetReached();
        StopAllNpcBehaviors();
        FinishQuestStep();
    }

    protected virtual void OnTargetReached() { }

    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;
            Debug.Log($"✓ QuestStep hoàn thành: QuestID={questId}, StepIndex={stepIndex}");
            StopAllNpcBehaviors();
            QuestManager.Instance.AdvanceQuestStep(questId);
            Destroy(this.gameObject);
        }
    }

    private void StopAllNpcBehaviors()
    {
        if (activeControllers != null)
        {
            foreach (var c in activeControllers)
            {
                if (c == null) continue;
                try { c.StopAllBehaviors(); } catch { }
            }
            activeControllers.Clear();
        }

        // Destroy spawned instances
        if (spawnedNpcInstances != null)
        {
            foreach (var go in spawnedNpcInstances)
            {
                if (go != null) Destroy(go);
            }
            spawnedNpcInstances.Clear();
        }
    }

    public void RegisterTargetIconIfPresent()
    {
        var icon = GetComponentInChildren<QuestTargetIcon>(true);
        if (icon != null)
        {
            icon.SetQuestAndStep(questId, stepIndex, this);
        }
        else
        {
            Debug.LogWarning($"QuestStep ({gameObject.name}) did not find a child QuestTargetIcon. Place an icon under the step prefab to allow arrival detection.");
        }
    }
}
