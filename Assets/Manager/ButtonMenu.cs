using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Simple component for UI Buttons to return to the Menu scene
public class ButtonMenu : MonoBehaviour
{
    [Tooltip("Name of the Menu scene to load. Change if your Menu scene has a different name.")]
    public string menuSceneName = "Menu";

    [Tooltip("Optional: assign the UI Button here to auto-register BackToMenu on click.")]
    public Button menuButton;

    // Hook this method to a UI Button OnClick()
    public void BackToMenu()
    {
        // Ensure game is unpaused immediately
        Time.timeScale = 1f;

        if (string.IsNullOrEmpty(menuSceneName))
        {
            Debug.LogWarning("ButtonMenu: menuSceneName is empty. Cannot load Menu scene.");
            return;
        }

        // Load asynchronously and activate immediately to ensure the scene switch happens right away
        var op = SceneManager.LoadSceneAsync(menuSceneName, LoadSceneMode.Single);
        if (op != null)
        {
            op.allowSceneActivation = true;
        }
    }

    private void Awake()
    {
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(BackToMenu);
        }
    }

    private void OnDestroy()
    {
        if (menuButton != null)
        {
            menuButton.onClick.RemoveListener(BackToMenu);
        }
    }
}
