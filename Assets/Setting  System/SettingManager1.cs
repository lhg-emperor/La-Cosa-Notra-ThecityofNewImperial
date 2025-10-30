using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Script quản lý giao diện Setting (Prefab):
/// - Tự căn giữa Setting UI khi bật.
/// - Button mở Setting được gán thủ công (chỉ dùng cho Scene đặc biệt).
/// - Scene đặc biệt được xác định bằng danh sách tên (SpecialSceneNames).
/// - Trong Scene thường → nhấn ESC để mở/tắt Setting.
/// - Trong Scene đặc biệt → chỉ mở qua Button.
/// - Khi Setting bật → game tạm dừng (Time.timeScale = 0).
/// - Khi đóng → game chạy lại bình thường.
/// </summary>
public class SettingManager : MonoBehaviour
{
    [Header("Prefab giao diện Setting UI")]
    public GameObject settingPrefab; // Prefab UI cài đặt

    [Header("Button mở Setting (chỉ dùng cho Scene đặc biệt)")]
    public Button openSettingButton; // Gán thủ công trong Menu

    [Header("Danh sách Scene đặc biệt (Menu, v.v.)")]
    public string[] specialSceneNames; // Ví dụ: {"MainMenu", "Home"}

    private GameObject settingInstance; // Instance của Setting UI
    private bool isSettingActive = false; // Trạng thái bật/tắt
    private bool isSpecialScene = false; // Cờ nội bộ, tự xác định theo tên Scene

    void Start()
    {
        // 🔹 Kiểm tra xem Scene hiện tại có nằm trong danh sách đặc biệt không
        string currentScene = SceneManager.GetActiveScene().name;
        foreach (string name in specialSceneNames)
        {
            if (currentScene == name)
            {
                isSpecialScene = true;
                break;
            }
        }

        // 🔹 Nếu có Button Setting → gán sự kiện thủ công
        if (openSettingButton != null)
        {
            openSettingButton.onClick.AddListener(OnSettingButtonPressed);
        }

        // 🔹 Khởi tạo Setting UI nếu có Prefab
        if (settingPrefab != null)
        {
            settingInstance = Instantiate(settingPrefab, transform);
            settingInstance.SetActive(false);

            // 🔸 Căn giữa giao diện Setting (trong Canvas)
            RectTransform rect = settingInstance.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Chưa gán Prefab Setting UI vào SettingManager!");
        }
    }

    void Update()
    {
        // 🔹 Nếu KHÔNG phải Scene đặc biệt → nhấn ESC để bật/tắt
        if (!isSpecialScene && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSetting();
        }

        // 🔹 Nếu đang ở Scene đặc biệt mà nhấn ESC khi đang mở Setting → thoát
        if (isSpecialScene && isSettingActive && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSetting();
        }
    }

    /// <summary>
    /// Được gọi khi người chơi nhấn Button "Setting" trong Scene đặc biệt.
    /// </summary>
    public void OnSettingButtonPressed()
    {
        if (isSpecialScene)
        {
            ToggleSetting();
        }
    }

    /// <summary>
    /// Bật/tắt Setting UI và xử lý tạm dừng game.
    /// </summary>
    private void ToggleSetting()
    {
        if (settingInstance == null)
        {
            Debug.LogWarning("⚠️ Không có Prefab Setting để bật/tắt!");
            return;
        }

        isSettingActive = !isSettingActive;
        settingInstance.SetActive(isSettingActive);

        // 🔹 Khi bật → dừng game, khi tắt → tiếp tục
        Time.timeScale = isSettingActive ? 0 : 1;
    }

    /// <summary>
    /// Gắn vào nút “Thoát” trong UI Setting để đóng Setting.
    /// </summary>
    public void CloseSetting()
    {
        if (settingInstance != null)
        {
            settingInstance.SetActive(false);
            isSettingActive = false;
            Time.timeScale = 1;
        }
    }
}
