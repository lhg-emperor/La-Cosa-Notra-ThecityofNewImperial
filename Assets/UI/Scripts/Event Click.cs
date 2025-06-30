using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISceneLoader : MonoBehaviour
{
    [Header("Story")]
    [SerializeField] private Button storyButton;
    [SerializeField] private string storySceneName = "StoryScene";

    [Header("Quit")]
    [SerializeField] private Button quitButton;

    void Start()
    {
        // Story
        if (storyButton != null)
        {
            storyButton.onClick.AddListener(LoadStoryScene);
        }
        else
        {
            Debug.LogWarning("Story Button chưa được gán!");
        }

        // Quit
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogWarning("Quit Button chưa được gán!");
        }
    }

    public void LoadStoryScene()
    {
        Debug.Log("Loading scene: " + storySceneName);
        SceneManager.LoadScene(storySceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Đang thoát game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Thoát Play Mode
#else
        Application.Quit(); // Thoát game thật
#endif
    }
}
