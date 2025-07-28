using UnityEngine;

public interface IGun: IWeapon
{
    void Fire(playerPickup owner);
}
