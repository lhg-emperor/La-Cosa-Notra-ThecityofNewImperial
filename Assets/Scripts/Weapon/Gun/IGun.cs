using UnityEngine;

public interface IGun : IWeapon
{
    void Fire(playerPickup owner);

    // Thuộc tính để lấy Icon của súng
    Sprite WeaponIcon { get; }

    // Hàm để thiết lập UIManager cho súng (để súng tự cập nhật UI khi bắn)
    void SetUIManager(UIManager manager);

    // Thuộc tính để lấy số đạn hiện tại trong băng đạn
    int CurrentMagazineAmmo { get; }

    // Thuộc tính để lấy tổng số đạn dự trữ
    int TotalReserveAmmo { get; }
}