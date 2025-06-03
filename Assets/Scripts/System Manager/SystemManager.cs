using UnityEngine;

public class GameSystemManager : MonoBehaviour
{
    public static GameSystemManager Instance { get; private set; }

    [Header("Player tham chiếu trong scene")]
    public Player player;

    // Các định nghĩa vũ khí sẵn có
    private WeaponManager emptyWeapon;
    private WeaponManager batWeapon;
    private WeaponManager katanaWeapon;

    void Awake()
    {
        // Đảm bảo Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Khởi tạo các vũ khí
        InitializeWeapons();
    }

    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("⚠️ Player chưa được gán trong GameSystemManager. Gán thủ công hoặc tìm tự động.");
        }
    }

    private void InitializeWeapons()
    {
        emptyWeapon = new EmptyWeapon();
        batWeapon = new BatWeapon();
        katanaWeapon = new KatanaWeapon();
    }

    public WeaponManager GetDefaultWeapon()
    {
        return emptyWeapon;
    }

    public WeaponManager GetWeaponByName(string name)
    {
        // Lấy vũ khí dựa trên tên gọi
        switch (name)
        {
            case "BaseballBat":
                return batWeapon;
            case "Katana":
                return katanaWeapon;
            case "Empty":
            default:
                return emptyWeapon;
        }
    }

    public void TriggerPlayerAttack()
    {
        player?.Attack();
    }
}
