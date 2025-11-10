using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISceneLoader : MonoBehaviour
{
    [Header("Story")]
    [SerializeField] private Button storyButton;
    [SerializeField] private string storySceneName = "MainScene";
    [SerializeField] private Button loadButton; 

    [Header("Quit")]
    [SerializeField] private Button quitButton;

    void Start()
    {
        // Gán sự kiện cho Story
        if (storyButton != null)
        {
            storyButton.onClick.AddListener(() =>
            {
                // Kiểm tra DataPersistenceManager trước khi gọi
                if (DataPersistenceManager.instance != null)
                {
                    DataPersistenceManager.instance.NewGame();
                }
                else
                {
                    Debug.LogWarning("DataPersistenceManager.instance chưa tồn tại! Hãy thêm manager vào scene đầu tiên và bật DontDestroyOnLoad.");
                }

                Debug.Log("Loading scene: " + storySceneName);
                SceneManager.LoadScene(storySceneName);
            });
        }
        else
        {
            Debug.LogWarning("Story Button chưa được gán!");
        }

        // Load
        if (loadButton != null)
        {
            loadButton.onClick.AddListener(LoadGame);
        }

        // Gán sự kiện cho Quit
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogWarning("Quit Button chưa được gán!");
        }

        //Xử lý vấn đề khi không có DataSave khiến Load Game trở thành New Game
        if(!DataPersistenceManager.instance.HasGameData())
        {
                loadButton.interactable = false;
                Debug.Log("Không tìm thấy dữ liệu lưu, vô hiệu hóa nút Load Game.");

        }
    }

    private void LoadGame()
    {
        SceneManager.LoadScene(storySceneName);
    }

    private void QuitGame()
    {
        Debug.Log("Đang thoát game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Thoát Play Mode
#else
        Application.Quit(); // Thoát game thật
#endif
    }
}
