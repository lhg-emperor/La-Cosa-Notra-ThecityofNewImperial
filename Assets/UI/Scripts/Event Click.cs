using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class UISceneLoader : MonoBehaviour
{
    [SerializeField] private Button storyButton; 
    [SerializeField] private string storySceneName = "StoryScene";

    void Start()
    {
        
        if (storyButton != null)
        {
            storyButton.onClick.AddListener(LoadStoryScene);
        }
        else
        {
            Debug.LogWarning("Story Button chưa được gán!");
        }
    }

    public void LoadStoryScene()
    {
        Debug.Log("Loading scene: " + storySceneName);
        SceneManager.LoadScene(storySceneName);
    }
}
