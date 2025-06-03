using UnityEngine;

public class BatWeapon : WeaponManager
{
    public override string WeaponName => "Baseball Bat";
    public override WeaponType Type => WeaponType.Melee;
    public override int Damage => 35;
    public override float Range => 1.2f;

    public override void PerformAttack(Player attacker)
    {
        // TODO: Thêm logic xử lý tấn công cho gậy bóng chày
        Debug.Log("Bat attack performed!");
    }

    public override void OnEquip(Player player)
    {
        // TODO: Gán animation, gắn vũ khí lên tay,...
        Debug.Log("Bat equipped.");
    }

    public override void OnUnequip(Player player)
    {
        // TODO: Reset animation, tháo vũ khí,...
        Debug.Log("Bat unequipped.");
    }
}
