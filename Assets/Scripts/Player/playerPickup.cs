using UnityEngine;
using System.Collections.Generic;

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
    public Animator animator; // << Giữ link Animator trực tiếp
    public bool IsAttacking = false; // << Dùng cho vũ khí

    void Awake()
    {
        CurrentDamage = baseDamage;
        animator = GetComponent<Animator>();
        defaultAnimator = animator.runtimeAnimatorController;
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

        // Đổi lại Animator gốc
        animator.runtimeAnimatorController = defaultAnimator;
    }

    public void SwitchWeapon(int direction)
    {
        if (weaponSlots.Count == 0) return;

        activeWeaponIndex += direction;
        if (activeWeaponIndex >= weaponSlots.Count) activeWeaponIndex = 0;
        if (activeWeaponIndex < 0) activeWeaponIndex = weaponSlots.Count - 1;

        IWeapon weapon = currentWeapon;
        if (weapon != null)
        {
            CurrentDamage = weapon.GetDamage();
            currentGun = weapon as IGun;


            animator.runtimeAnimatorController = weapon.GetAnimatorController();
        }
    }
}
