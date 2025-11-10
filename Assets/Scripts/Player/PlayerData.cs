using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
 public int Health;
 public Vector3 Position;

 // Thông tin v? v? khí hi?n t?i
 public string CurrentWeapon; // l?u tên ki?u v? khí
 public int CurrentMagazineAmmo;
 public int TotalReserveAmmo;

 // Danh sách v? khí (l?u tên ki?u)
 public List<string> WeaponSlots = new List<string>();

 // Tr?ng thái ?ang ? trên xe
 public bool IsInVehicle;

 public PlayerData() { }

 // T?o PlayerData t? ??i t??ng Player hi?n t?i (??c các component công khai)
 public static PlayerData FromPlayer(Player player)
 {
 var data = new PlayerData();
 if (player == null) return data;

 data.Health = Mathf.RoundToInt(player.Health);
 data.Position = player.transform.position;

 var pickup = player.GetComponent<playerPickup>();
 if (pickup != null)
 {
 data.CurrentWeapon = pickup.currentGun != null ? pickup.currentGun.GetType().Name : null;
 try
 {
 data.CurrentMagazineAmmo = pickup.currentGun != null ? pickup.currentGun.CurrentMagazineAmmo :0;
 data.TotalReserveAmmo = pickup.currentGun != null ? pickup.currentGun.TotalReserveAmmo :0;
 }
 catch
 {
 data.CurrentMagazineAmmo =0;
 data.TotalReserveAmmo =0;
 }

 data.WeaponSlots = pickup.weaponSlots != null
 ? pickup.weaponSlots.Select(w => w != null ? w.GetType().Name : string.Empty).ToList()
 : new List<string>();
 }

 // Ph?ng ?oán isInVehicle b?ng cách ki?m tra SpriteRenderer.enabled (player b??n khi vào xe trong project này)
 var sr = player.GetComponent<SpriteRenderer>();
 data.IsInVehicle = sr != null ? !sr.enabled : false;

 return data;
 }

 // Áp d?ng d? li?u tr? l?i Player (c? g?ng c?p nh?t nh?ng gì có th?)
 public void ApplyToPlayer(Player player)
 {
 if (player == null) return;

 player.Health = Health;
 player.transform.position = Position;

 var pickup = player.GetComponent<playerPickup>();
 if (pickup != null)
 {
 // Reset damage base và c? g?ng ??t l?i currentGun n?u tìm th?y trong scene
 pickup.CurrentDamage = pickup.baseDamage;

 if (!string.IsNullOrEmpty(CurrentWeapon))
 {
 // Tìm1 IGun trong scene có ki?u tên gi?ng CurrentWeapon
 var allGuns = Object.FindObjectsOfType<MonoBehaviour>().OfType<IGun>();
 var gun = allGuns.FirstOrDefault(g => g.GetType().Name == CurrentWeapon);
 if (gun != null)
 {
 pickup.currentGun = gun;
 pickup.CurrentDamage = gun.GetDamage();
 }
 }
 }

 var sr = player.GetComponent<SpriteRenderer>();
 if (sr != null)
 sr.enabled = !IsInVehicle; // n?u IsInVehicle=true thì?n sprite
 }
}
