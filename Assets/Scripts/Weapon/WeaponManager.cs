using UnityEngine;

public enum WeaponType
{
    Empty,
    Melee,
    Ranged,
    Explosive
}

public abstract class WeaponManager
{
    public abstract string WeaponName { get; }
    public abstract WeaponType Type { get; }
    public abstract int Damage { get; }
    public abstract float Range { get; }

    /// <summary>
    /// Gọi hành vi tấn công (Animation, sát thương, v.v.)
    /// </summary>
    public abstract void PerformAttack(Player attacker);

    /// <summary>
    /// Gọi khi trang bị vũ khí này cho Player
    /// </summary>
    public virtual void OnEquip(Player player)
    {
        Debug.Log($"[Weapon] {WeaponName} được trang bị cho {player.name}");
    }

    /// <summary>
    /// Gọi khi bỏ trang bị
    /// </summary>
    public virtual void OnUnequip(Player player)
    {
        Debug.Log($"[Weapon] {WeaponName} bị gỡ khỏi {player.name}");
    }

    /// <summary>
    /// (Tuỳ chọn) Nếu muốn dùng mô hình ảo để attach vào Player
    /// </summary>
    
    public virtual GameObject GetWeaponModelPrefab()
    {
        return null; // Mặc định: không có mô hình
    }

    /// <summary>
    /// (Tuỳ chọn) Nếu cần trigger animation trong Animator của Player
    /// </summary>
    public virtual string GetAttackAnimationTrigger()
    {
        return string.Empty;
    }
}
