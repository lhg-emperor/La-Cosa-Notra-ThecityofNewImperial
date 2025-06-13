using UnityEngine;

public class playerPickup : MonoBehaviour
{
    public float baseDamage = 10f;
    public float CurrentDamage { get; private set; }

    private IWeapon currentWeapon;
    private GameObject nearWeapon;

    void Awake()
    {
        CurrentDamage = baseDamage;
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
        if (nearWeapon != null)
        {
            IWeapon weapon = nearWeapon.GetComponent<IWeapon>();
            if (weapon != null && weapon.CanPickUp)
            {
                weapon.OnPickUp();
                currentWeapon = weapon;
                CurrentDamage = weapon.GetDamage();

                string weaponName = weapon.GetType().Name;
                GetComponent<Player>()?.SetWeaponType(weaponName);
                var animCtrl = weapon.GetAnimatorController();
                GetComponent<Animator>().runtimeAnimatorController = animCtrl;

                nearWeapon = null;
            }
        }
    }

    public void Drop()
    {
        if (currentWeapon != null)
        {
            Vector3 dropPos = transform.position + transform.right * 1f;
            currentWeapon.OnDrop(dropPos);
            currentWeapon = null;
            CurrentDamage = baseDamage;

            RuntimeAnimatorController VitoAnim = Resources.Load<RuntimeAnimatorController>("Animations/Main-Vito/Vito");
            // ⚠ Gán lại Animator mặc định nếu cần:
            // GetComponent<Animator>().runtimeAnimatorController = VitoAnim;
            // GetComponent<Player>()?.SetWeaponType("");
        }
    }
}
