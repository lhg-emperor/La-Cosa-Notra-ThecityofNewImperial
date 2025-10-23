using UnityEngine;
using System.Collections;
using System;

public class Piston : MonoBehaviour, IGun
{
    [Header("Piston Setting")]
    public float PisDamage = 21.5f;
    public int ammo = 30; // Đạn hiện tại trong băng đạn
    public bool canPickUp = false;

    public int totalReserveAmmo = 30;

    public RuntimeAnimatorController pistonAnimator;
    public GameObject bulletPre;
    public Transform firePoint;

    [Header("UI & Logic")]
    public Sprite iconSprite;
    private UIManager uiManager;

    // Triển khai thuộc tính cơ bản
    public bool CanPickUp => canPickUp;
    public float GetDamage() => PisDamage;

    public Sprite WeaponIcon => iconSprite;
    public void SetUIManager(UIManager manager)
    {
        uiManager = manager;
    }
    public int CurrentMagazineAmmo => ammo;
    public int TotalReserveAmmo => totalReserveAmmo;
    
    public void OnPickUp()
    {
        gameObject.SetActive(false);
    }

    public void OnDrop(Vector3 dropPosition)
    {
        transform.position = dropPosition;
        gameObject.SetActive(true);
    }

    public RuntimeAnimatorController GetAnimatorController()
    {
        return pistonAnimator;
    }

    public void Fire(playerPickup owner)
    {
        if (ammo <= 0) return;
        if (owner.IsAttacking) return;

        owner.IsAttacking = true;
        owner.animator.SetBool("isShoot", true);
        owner.StartCoroutine(FireAfterAnimation(owner));
    }

    private IEnumerator FireAfterAnimation(playerPickup owner)
    {
        yield return new WaitForSeconds(0.48f);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - owner.transform.position).normalized;

        GameObject bullet = Instantiate(bulletPre, firePoint.position, Quaternion.identity);
        PistonBullet bulletScript = bullet.GetComponent<PistonBullet>();
        bulletScript.Initialize(dir, PisDamage, owner.GetComponent<Collider2D>());


        ammo--; // ĐẠN GIẢM!

        // GỌI CẬP NHẬT UI SAU KHI BẮN
        if (uiManager != null)
        {
            uiManager.UpdateWeaponDisplay(WeaponIcon, CurrentMagazineAmmo, TotalReserveAmmo);
        }

        owner.animator.SetBool("isShoot", false);
        owner.IsAttacking = false;

        if (ammo <= 0)
        {
            // Xử lý khi hết đạn
            owner.currentGun = null;
            owner.CurrentDamage = owner.baseDamage;
            owner.animator.runtimeAnimatorController = owner.defaultAnimator;

            // CẬP NHẬT UI về trạng thái tay không
            if (uiManager != null && uiManager.defaultIcon != null)
            {
                uiManager.UpdateWeaponDisplay(uiManager.defaultIcon, 0, 0);
            }

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($" Piston OnTriggerEnter2D với {collision.name}");
            canPickUp = true;
            collision.GetComponent<playerPickup>()?.SetNearWeapon(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canPickUp = false;
            collision.GetComponent<playerPickup>()?.ClearNearWeapon(gameObject);
        }
    }
}