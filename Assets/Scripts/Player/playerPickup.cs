using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class playerPickup : MonoBehaviour
{
    public float baseDamage = 10f;
    public float CurrentDamage { get; set; }

    public List<IWeapon> weaponSlots = new List<IWeapon>();
    public int activeWeaponIndex = -1;

    public IWeapon currentWeapon =>
        activeWeaponIndex >= 0 && activeWeaponIndex < weaponSlots.Count
        ? weaponSlots[activeWeaponIndex]
        : null;

    public IGun currentGun;
    private GameObject nearWeapon;

    public RuntimeAnimatorController defaultAnimator;
    public Animator animator;
    public bool IsAttacking = false;

    private UIManager uiManager;

    void Awake()
    {
        CurrentDamage = baseDamage;
        animator = GetComponent<Animator>();
        defaultAnimator = animator.runtimeAnimatorController;

        uiManager = FindObjectOfType<UIManager>();
    }

    public void SetNearWeapon(GameObject weapon)
    {
        if (weapon.GetComponent<IWeapon>() != null)
            nearWeapon = weapon;
    }

    public void ClearNearWeapon(GameObject weapon)
    {
        if (nearWeapon == weapon)
            nearWeapon = null;
    }

    public void PickUp()
    {
        if (nearWeapon == null) return;

        IWeapon weapon = nearWeapon.GetComponent<IWeapon>();
        if (weapon == null || !weapon.CanPickUp) return;

        if (weaponSlots.Count >= 2) return;

        weapon.OnPickUp();
        weaponSlots.Add(weapon);
        nearWeapon = null;

        SwitchWeapon(0);
    }

    public void Drop()
    {
        if (currentWeapon == null) return;

        Vector3 dropPos = transform.position + transform.right * 1f;
        currentWeapon.OnDrop(dropPos);

        weaponSlots.RemoveAt(activeWeaponIndex);
        activeWeaponIndex = -1;

        currentGun = null;
        CurrentDamage = baseDamage;

        animator.runtimeAnimatorController = defaultAnimator;

        UpdateWeaponUI(null);
    }

    public void SwitchWeapon(int direction)
    {
        if (weaponSlots.Count == 0)
        {
            activeWeaponIndex = -1;
            ResetToDefault();
            return;
        }

        activeWeaponIndex += direction;

        if (activeWeaponIndex > weaponSlots.Count - 1)
            activeWeaponIndex = -1;
        if (activeWeaponIndex < -1)
            activeWeaponIndex = weaponSlots.Count - 1;

        IWeapon weapon = currentWeapon;
        if (weapon != null)
        {
            CurrentDamage = weapon.GetDamage();
            currentGun = weapon as IGun;
            animator.runtimeAnimatorController = weapon.GetAnimatorController();

            UpdateWeaponUI(weapon as IGun);
        }
        else
        {
            ResetToDefault();
            UpdateWeaponUI(null);
        }
    }

    private void UpdateWeaponUI(IGun gun)
    {
        if (uiManager == null) return;

        if (gun != null)
        {
            gun.SetUIManager(uiManager);

            // Lấy đạn một cách linh hoạt từ thuộc tính mới của IGun
            int currentMag = gun.CurrentMagazineAmmo;
            int totalReserve = gun.TotalReserveAmmo;

            uiManager.UpdateWeaponDisplay(gun.WeaponIcon, currentMag, totalReserve);
        }
        else
        {
            uiManager.UpdateWeaponDisplay(uiManager.defaultIcon, 0, 0);
        }
    }

    void ResetToDefault()
    {
        CurrentDamage = baseDamage;
        currentGun = null;
        animator.runtimeAnimatorController = defaultAnimator;
    }
}