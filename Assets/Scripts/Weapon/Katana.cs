﻿using UnityEngine;
using UnityEngine.InputSystem;

public class Katana : MonoBehaviour, IWeapon
{
    public int damage = 17;
    public bool canPickUp = false;
    public GameObject KatanaPrefab;

    public RuntimeAnimatorController KatanaAnimator;

    public int GetDamage() => damage;
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
        Instantiate(KatanaPrefab, dropPosition, Quaternion.identity);
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
        return KatanaAnimator;
    }
}
