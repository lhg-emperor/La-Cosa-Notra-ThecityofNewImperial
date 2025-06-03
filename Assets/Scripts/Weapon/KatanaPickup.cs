using UnityEngine;

public class KatanaPickup : MonoBehaviour
{
    private Player player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<Player>();
            if (player != null)
            {
                player.SetNearbyWeaponPickup(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (player != null)
            {
                player.SetNearbyWeaponPickup(null);
                player = null;
            }
        }

    }

    public void GiveKatana()
    {
        if (player == null)
        {
            Debug.LogError("[KatanaPickup] Không tìm thấy player để trao Katana.");
            return;
        }

        Debug.Log("[KatanaPickup] Đưa Katana cho player...");
        KatanaWeapon katana = new KatanaWeapon();
        player.EquipWeapon(katana);

        // Reset thông tin trong Player trước khi hủy vật thể
        player.SetNearbyWeaponPickup(null);
        player = null;

        Debug.Log("[KatanaPickup] Katana đã được nhặt. Hủy vật thể trong game.");
        Destroy(gameObject);

    }
}
