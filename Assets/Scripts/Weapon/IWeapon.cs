using UnityEngine;

public interface IWeapon
{
    bool CanPickUp { get; }
    float GetDamage();
    void OnPickUp();
    void OnDrop(Vector3 dropPosition);
    RuntimeAnimatorController GetAnimatorController();
}
