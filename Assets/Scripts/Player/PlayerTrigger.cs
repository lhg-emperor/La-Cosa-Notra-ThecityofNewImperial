using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private List<IDamageable> targets = new List<IDamageable>();
    public List<IDamageable> GetTargets() => targets;

    private void OnTriggerEnter2D(Collider2D npc)
    {
        IDamageable target = npc.GetComponent<IDamageable>();
        if (target != null && !targets.Contains(target))
        {
            targets.Add(target);
            Debug.Log("OnTriggerEnter");
        }
    }

    private void OnTriggerExit2D(Collider2D npc)
    {
        IDamageable target = npc.GetComponent<IDamageable>();
        if (target != null && targets.Contains(target))
        {
            targets.Remove(target);
        }
    }
}
