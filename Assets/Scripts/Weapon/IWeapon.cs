using UnityEngine;
public interface IWeapon
{
    int GetDamage();
    bool CanPickUp { get; }
    void OnPickUp();
    void OnDrop(Vector3 dropPosition);
}
