using UnityEngine;

public class OptionManager : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel;
    private bool isOpen = false;
    public bool OptionVisible => isOpen;
    private bool previousCursorVisible = true;
    private CursorLockMode previousLockState = CursorLockMode.None;

    void Awake()
    {
        if (optionPanel != null)
            optionPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleOptions();
    }

    public void ToggleOptions()
    {
        SetOptions(!isOpen);
    }

    public void SetOptions(bool show)
    {
        if (optionPanel == null)
        {
            Debug.LogWarning("OptionManager: optionPanel chưa được gán trong Inspector.");
            return;
        }

        isOpen = show;
        optionPanel.SetActive(isOpen);

        if (isOpen)
        {
            // Save current cursor state so we can restore it when closing
            previousCursorVisible = Cursor.visible;
            previousLockState = Cursor.lockState;

            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            // Restore the cursor state that was active before opening the options
            Cursor.visible = previousCursorVisible;
            Cursor.lockState = previousLockState;
        }
    }

    public void CloseOptions()
    {
        if (isOpen) SetOptions(false);
    }
}
