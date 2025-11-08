using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;

    void Update()
    {
        // Nhấn ESC để pause/resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f; // Dừng game
        isPaused = true;
        Debug.Log("Game Paused");
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f; // Tiếp tục game
        isPaused = false;
        Debug.Log("Game Resumed");
    }
}
