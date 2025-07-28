using UnityEngine;
using UnityEngine.InputSystem;

public class Bat : MonoBehaviour, IWeapon
{
    [Header("Weapon Setting")]
    public float damage = 15;
    public bool canPickUp = false;

    public RuntimeAnimatorController BatAnimator;

    public float GetDamage() => damage;
    public bool CanPickUp => canPickUp;

    public PlayerControls playerControls;
    public InputAction pickupAction;
    public InputAction dropAction;

    public void OnPickUp()
    {
        Destroy(gameObject); 
    }

    public void OnDrop(Vector3 dropPosition)
    {
        GameObject prefab = Resources.Load<GameObject>("Weapons/Bat");
        GameObject clone = Object.Instantiate(prefab, dropPosition, Quaternion.identity);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canPickUp = true;
            collision.gameObject
                .GetComponent<playerPickup>()
                ?.SetNearWeapon(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canPickUp = false;
            collision.gameObject
                .GetComponent<playerPickup>()
                ?.ClearNearWeapon(gameObject);
        }
    }

    public RuntimeAnimatorController GetAnimatorController()
    {
        return BatAnimator;
    }
}
