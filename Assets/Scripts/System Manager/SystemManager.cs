using UnityEngine;

public class GameSystemManager : MonoBehaviour
{
    public static GameSystemManager Instance { get; private set; }

    [Header("Player reference")]
    public Player player;

    [Header("Weapon prefabs or objects")]
    public GameObject baseballBatPrefab;
    public GameObject katanaPrefab;

    public IWeapon emptyWeapon;
    public IWeapon batWeapon;
    public IWeapon katanaWeapon;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Khởi tạo các vũ khí
        emptyWeapon = new EmptyWeapon();
        batWeapon = new BatWeapon(baseballBatPrefab);
        katanaWeapon = new KatanaWeapon(katanaPrefab);
    }

    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("⚠️ GameSystemManager chưa được gán Player! Gán trong Inspector hoặc tìm trong Scene.");
        }
    }

    public IWeapon GetDefaultWeapon()
    {
        return emptyWeapon;
    }

    public IWeapon GetWeaponByName(string name)
    {
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
        if (player != null)
        {
            player.Attack();
        }
    }
}
