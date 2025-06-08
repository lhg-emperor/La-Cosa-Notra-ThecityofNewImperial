using UnityEngine;

public class Katana : MonoBehaviour
{
    public float KatDamage = 17f;
    public bool canPickUp = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canPickUp = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canPickUp = false;
        }
    }
}
