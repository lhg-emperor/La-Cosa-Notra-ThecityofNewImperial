using UnityEngine;

public class KatanaWeapon : WeaponManager
{

    public override string WeaponName => "Katana";
    public override WeaponType Type => WeaponType.Melee;
    public override int Damage => 40;
    public override float Range => 1.5f;

    public override void PerformAttack(Player attacker)
    {
        // TODO: Triển khai logic tấn công thực tế ở đây
        Debug.Log($"{attacker.name} vung Katana gây {Damage} sát thương!");
    }

    public override void OnEquip(Player player)
    {
        base.OnEquip(player);
        // TODO: Load mô hình, set animation,...
        Debug.Log($"{player.name} đã trang bị Katana.");
    }

    public override void OnUnequip(Player player)
    {
        base.OnUnequip(player);
        // TODO: Gỡ mô hình, reset animation,...
        Debug.Log($"{player.name} đã gỡ bỏ Katana.");
    }


    public override string GetAttackAnimationTrigger()
    {
        return "Attack_Katana"; // Phải trùng với tên trigger trong Animator
    }

}
