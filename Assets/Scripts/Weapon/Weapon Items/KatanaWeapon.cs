using UnityEngine;

public class KatanaWeapon : IWeapon
{
    private GameObject prefab;

    public KatanaWeapon(GameObject prefab)
    {
        this.prefab = prefab;
    }

    public string WeaponName => "Katana";
    public WeaponType Type => WeaponType.Melee;
    public int Damage => 50;
    public float Range => 1.5f;
    public Sprite Icon => null;
    public GameObject WeaponModelPrefab => prefab;

    public string PlayerIdleAnimState => "Idle_Katana";
    public string PlayerRunAnimState => "Run_Katana";
    public string PlayerAttackAnimTrigger => "Slash_Katana";

    public void PerformAttack(Player attacker)
    {
        Debug.Log("Chém bằng katana!");
    }

    public void OnEquip(Player player, Transform handTransform)
    {
        if (prefab != null && handTransform != null)
        {
            GameObject weaponInstance = GameObject.Instantiate(prefab, handTransform);
        }
    }

    public void OnUnequip(Player player)
    {
        // TODO: clear vũ khí cũ
    }
}
