// IWeapon.cs
using UnityEngine;

// Enum WeaponType giữ nguyên
public enum WeaponType
{
    Empty,
    Melee,
    Ranged,
    Explosive
}

public interface IWeapon
{
    string WeaponName { get; }
    WeaponType Type { get; }
    int Damage { get; }
    float Range { get; }
    Sprite Icon { get; }

    // Sẽ là Prefab chứa SpriteRenderer hoặc các đối tượng 2D khác.
    GameObject WeaponModelPrefab { get; }

    // Tên animation state/trigger trong Animator của Player (vẫn giữ nguyên)
    string PlayerIdleAnimState { get; }
    string PlayerRunAnimState { get; }
    string PlayerAttackAnimTrigger { get; }

    // Hành vi của vũ khí (vẫn giữ nguyên chữ ký)
    void PerformAttack(Player attacker);
    void OnEquip(Player player, Transform handTransform);
    void OnUnequip(Player player);
}