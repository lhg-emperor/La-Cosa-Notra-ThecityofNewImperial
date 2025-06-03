using UnityEngine;

public class EmptyWeapon : WeaponManager
{
    public override string WeaponName => "Empty";
    public override WeaponType Type => WeaponType.Empty;
    public override int Damage => 5;
    public override float Range => 1f;

    public override void OnEquip(Player player)
    {
        base.OnEquip(player);
        Debug.Log($"[EmptyWeapon] {player.name} đã trang bị tay không. Loại: {Type}, Sát thương: {Damage}, Tầm đánh: {Range}");
    }

    public override void OnUnequip(Player player)
    {
        base.OnUnequip(player);
        Debug.Log($"[EmptyWeapon] {player.name} gỡ bỏ tay không.");
    }

    public override void PerformAttack(Player attacker)
    {
        Debug.Log($"[EmptyWeapon] {attacker.name} tấn công tay không, gây {Damage} sát thương, tầm {Range}m.");
    }

}
