using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("Prefab giao diện Setting UI")]
    public GameObject settingPrefab;

    [Header("Button mở Setting (chỉ dùng cho Scene đặc biệt)")]
    public Button openSettingButton;

    [Header("Button đóng Setting (nút X trong Setting UI)")]
    public Button closeSettingButton;

    [Header("Danh sách Scene đặc biệt (Menu, v.v.)")]
    public string[] specialSceneNames;

    private GameObject settingInstance;
    private bool isSettingActive = false;
    private bool isSpecialScene = false;

    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        foreach (string name in specialSceneNames)
        {
            if (currentScene == name)
            {
                isSpecialScene = true;
                break;
            }
        }

        if (openSettingButton != null)
            openSettingButton.onClick.AddListener(OnSettingButtonPressed);

        if (settingPrefab != null)
        {
            settingInstance = Instantiate(settingPrefab, transform);
            settingInstance.SetActive(false);

            RectTransform rect = settingInstance.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
            }

            // 🔸 Tự tìm nút Close nếu chưa gán
            if (closeSettingButton == null)
            {
                Button[] buttons = settingInstance.GetComponentsInChildren<Button>(true);
                foreach (Button btn in buttons)
                {
                    if (btn.name.ToLower().Contains("close") || btn.name.ToLower().Contains("exit"))
                    {
                        closeSettingButton = btn;
                        break;
                    }
                }
            }

            // 🔹 Gán sự kiện đóng Setting
            if (closeSettingButton != null)
            {
                closeSettingButton.onClick.AddListener(CloseSetting);
                closeSettingButton.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("⚠️ Không tìm thấy nút Close trong Prefab Setting!");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Chưa gán Prefab Setting UI vào SettingManager!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSetting();
        }
    }

    public void OnSettingButtonPressed()
    {
        if (isSettingActive)
            return;

        ToggleSetting();
        Debug.Log("Button Setting Pressed");
    }

    private void ToggleSetting()
    {
        if (settingInstance == null)
        {
            Debug.LogWarning("⚠️ Không có Prefab Setting để bật/tắt!");
            return;
        }

        isSettingActive = !isSettingActive;
        settingInstance.SetActive(isSettingActive);

        if (closeSettingButton != null)
            closeSettingButton.gameObject.SetActive(isSettingActive);

        // 🔸 Chỉ Pause nếu Scene nằm trong danh sách đặc biệt
        if (isSpecialScene)
        {
            if (isSettingActive)
            {
                Time.timeScale = 0;
                if (openSettingButton != null)
                    openSettingButton.interactable = false;
            }
            else
            {
                Time.timeScale = 1;
                if (openSettingButton != null)
                    openSettingButton.interactable = true;
            }
        }
    }

    public void CloseSetting()
    {
        Debug.Log("⚙️ Close Button Clicked!");

        if (settingInstance != null)
        {
            settingInstance.SetActive(false);
            isSettingActive = false;

            // 🔸 Chỉ Resume nếu là Scene đặc biệt
            if (isSpecialScene)
                Time.timeScale = 1;

            if (closeSettingButton != null)
                closeSettingButton.gameObject.SetActive(false);

            if (openSettingButton != null)
                openSettingButton.interactable = true;
        }
    }
}
