// GameSystemManager.cs
using UnityEngine;

public class GameSystemManager : MonoBehaviour
{
    public static GameSystemManager Instance { get; private set; }
    public Weapon Empty;

    [Header("References")]
    public Player player;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (Empty == null)
        {
            Empty = new Weapon("Tay không", Weapon.WeaponType.Empty, 5, 1f);
        }
    }

    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("⚠️ GameSystemManager chưa được gán Player! Gán trong Inspector hoặc tìm trong Scene.");
        }
    }

    // trả về vũ khí mặc định (tay không)
    public Weapon GetDefaultWeapon()
    {
        return Empty;
    }

    public void TriggerPlayerAttack()
    {
        if (player != null)
        {
            player.Attack();
        }
    }
}
