using System.Collections;
using UnityEngine;

public class CitizenAI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private enum State
    {
        Roaming
    }
    private State state;
    private Citizen citizen;

    private void Awake()
    {
        citizen = GetComponent<Citizen>();
        state = State.Roaming;
    }
    private void Start()
    {
        StartCoroutine(RoamingRoutine());
    }
    private IEnumerator RoamingRoutine()
    {
        while(state == State.Roaming)
        {
            Vector2 roamPosition = GetRoamingPosition();
            citizen.Moveto(roamPosition);
            yield return new WaitForSeconds(6f);
        }
    }
    private Vector2 GetRoamingPosition()
    {
        return new Vector2(Random.Range(-1f,1f), Random.Range(-1f,1f)).normalized;
    }
}
