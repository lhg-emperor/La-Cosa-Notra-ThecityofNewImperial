using UnityEngine;

public class BatWeapon : IWeapon
{
    private GameObject prefab;

    public BatWeapon(GameObject prefab)
    {
        this.prefab = prefab;
    }

    public string WeaponName => "Gậy bóng chày";
    public WeaponType Type => WeaponType.Melee;
    public int Damage => 35;
    public float Range => 1.2f;
    public Sprite Icon => null;
    public GameObject WeaponModelPrefab => prefab;

    public string PlayerIdleAnimState => "Idle_Bat";
    public string PlayerRunAnimState => "Run_Bat";
    public string PlayerAttackAnimTrigger => "Swing_Bat";

    public void PerformAttack(Player attacker)
    {
        Debug.Log("Vung gậy bóng chày!");
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
