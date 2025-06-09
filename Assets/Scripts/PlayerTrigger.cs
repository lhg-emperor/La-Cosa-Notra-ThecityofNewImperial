using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerTrigger : MonoBehaviour
{
    private List<Citizen> targets = new List<Citizen>();
    public List<Citizen> GetTargets() => targets;
    private void OnTriggerEnter2D(Collider2D npc)
    {
        Citizen citizen = npc.GetComponent<Citizen>();
        if(citizen != null&& !targets.Contains(citizen))
        {
            targets.Add(citizen);
            Debug.Log(message: "OnTriggerEnter");
        } 
    }
    private void OnTriggerExit2D(Collider2D npc)
    {
        Citizen citizen = npc.GetComponent<Citizen>();
        if(citizen==null&&targets.Contains(citizen))
        {
            targets.Remove(citizen);
        }
    }
}
