using UnityEngine;

public class EmptyWeapon : IWeapon
{
    public string WeaponName => "Tay không";
    public WeaponType Type => WeaponType.Empty;
    public int Damage => 5;
    public float Range => 1f;
    public Sprite Icon => null;
    public GameObject WeaponModelPrefab => null;

    public string PlayerIdleAnimState => "Idle_Unarmed";
    public string PlayerRunAnimState => "Run_Unarmed";
    public string PlayerAttackAnimTrigger => "Punch";

    public void PerformAttack(Player attacker)
    {
        Debug.Log("Đấm tay không!");
    }

    public void OnEquip(Player player, Transform handTransform) { }
    public void OnUnequip(Player player) { }
}
