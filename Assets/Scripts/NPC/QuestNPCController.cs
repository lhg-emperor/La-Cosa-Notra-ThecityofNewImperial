using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// QuestNPCController manages NPC behaviors during quests: movement, combat, etc.
/// Attach this to an NPC GameObject and configure behaviors in QuestStep.
/// </summary>
[RequireComponent(typeof(Transform))]
public class QuestNPCController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Citizen citizen;
    private Coroutine activeRoutine;

    // Behavior types
    public enum BehaviorType { None, MoveTo, Attack }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        citizen = GetComponent<Citizen>();
    }

    /// <summary>
    /// Move NPC to destination. Pauses if player too far away.
    /// </summary>
    public void StartMoveTo(Transform destination, float pauseDistance = 6f, float resumeDistance = 4f, 
        float arrivalRadius = 1.25f, float checkInterval = 0.25f, Action onArrived = null)
    {
        StopAllBehaviors();
        activeRoutine = StartCoroutine(MoveToRoutine(destination, pauseDistance, resumeDistance, arrivalRadius, checkInterval, onArrived));
    }

    /// <summary>
    /// NPC attacks the player.
    /// </summary>
    public void StartAttack(Transform playerTransform, Action onAttackEnd = null)
    {
        StopAllBehaviors();
        activeRoutine = StartCoroutine(AttackRoutine(playerTransform, onAttackEnd));
    }

    /// <summary>
    /// Stop all active behaviors.
    /// </summary>
    public void StopAllBehaviors()
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }
        if (agent != null) agent.isStopped = true;
    }

    private IEnumerator MoveToRoutine(Transform destination, float pauseDistance, float resumeDistance, 
        float arrivalRadius, float checkInterval, Action onArrived)
    {
        if (destination == null) yield break;

        // Start movement
        if (agent != null)
        {
            agent.SetDestination(destination.position);
            agent.isStopped = false;
        }
        else if (citizen != null)
        {
            citizen.MoveTo(new Vector2(destination.position.x, destination.position.y), false);
        }

        // Find player
        Transform playerTransform = null;
        var go = GameObject.FindWithTag("Player");
        if (go != null) playerTransform = go.transform;
        else
        {
            var p = FindObjectOfType<Player>();
            if (p != null) playerTransform = p.transform;
        }

        // Movement loop
        while (true)
        {
            if (this == null || gameObject == null) yield break;
            if (destination == null) yield break;

            // Check arrival
            if (Vector3.Distance(transform.position, destination.position) <= arrivalRadius)
            {
                if (agent != null) agent.isStopped = true;
                onArrived?.Invoke();
                yield break;
            }

            // Pause/resume based on player distance
            if (playerTransform != null)
            {
                float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                if (distToPlayer > pauseDistance)
                {
                    if (agent != null) agent.isStopped = true;
                }
                else if (distToPlayer <= resumeDistance)
                {
                    if (agent != null)
                    {
                        agent.isStopped = false;
                        agent.SetDestination(destination.position);
                    }
                    else if (citizen != null)
                    {
                        citizen.MoveTo(new Vector2(destination.position.x, destination.position.y), false);
                    }
                }
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private IEnumerator AttackRoutine(Transform playerTransform, Action onAttackEnd)
    {
        if (playerTransform == null) yield break;

        // TODO: Implement attack logic
        // - NPC targets player
        // - Uses weapon/melee attack
        // - Stops when player dies or callback fires
        Debug.Log($"{gameObject.name} StartAttack({playerTransform.name}) - attack logic not yet implemented");

        // Placeholder: yield for 1 frame
        yield return null;
        onAttackEnd?.Invoke();
    }
}
