using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeCountManager : MonoBehaviour, IDataPersitence
{
    public static TimeCountManager Instance { get; private set; }

    [Header("UI Hiển thị thời gian (Text hoặc TMP)")]
    [SerializeField] private Text uiText;
    [SerializeField] private TextMeshProUGUI tmpText;

    private float timeCount = 0f;
    private bool isCounting = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Ensure we call DontDestroyOnLoad on the root GameObject.
        // If this component sits on a child, calling DontDestroyOnLoad(this.gameObject)
        // triggers a warning. Using transform.root.gameObject is safe in both cases.
        DontDestroyOnLoad(transform.root.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadData(GameData data)
    {
        this.timeCount = data.timeCount;
    }

    public void SaveData(ref GameData data)
    {
        data.timeCount = Mathf.FloorToInt(this.timeCount);
    }

    private void Update()
    {
        if (isCounting)
        {
            timeCount += Time.deltaTime;
            UpdateUIText();
        }
    }

    private void UpdateUIText()
    {
        string formatted = GetFormattedPlayTime();

        if (uiText != null)
            uiText.text = formatted;
        if (tmpText != null)
            tmpText.text = formatted;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (uiText == null && tmpText == null)
        {
            uiText = Object.FindFirstObjectByType<Text>();
            tmpText = Object.FindFirstObjectByType<TextMeshProUGUI>();
        }
        UpdateUIText();
    }

    public void PauseTime() => isCounting = false;
    public void ResumeTime() => isCounting = true;

    public string GetFormattedPlayTime()
    {
        int hours = Mathf.FloorToInt(timeCount / 3600);
        int minutes = Mathf.FloorToInt((timeCount % 3600) / 60);
        int seconds = Mathf.FloorToInt(timeCount % 60);
        return $"{hours:00}:{minutes:00}:{seconds:00}";
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
