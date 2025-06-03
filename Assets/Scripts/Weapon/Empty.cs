using UnityEngine;

[System.Serializable]
public class Weapon
{
    public enum WeaponType
    {
        Empty,   // Tay không
        Melee,      // Dao, gậy,...
        Ranged,     // Súng
        Explosive   // Lựu đạn, bom,...
    }

    public string weaponName;
    public WeaponType type;
    public int damage;
    public float range;
    public Sprite icon;

    public Weapon(string name, WeaponType type, int damage, float range, Sprite icon = null)
    {
        this.weaponName = name;
        this.type = type;
        this.damage = damage;
        this.range = range;
        this.icon = icon;
    }
}
