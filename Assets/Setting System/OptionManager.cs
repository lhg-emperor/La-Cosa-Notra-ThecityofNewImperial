using UnityEngine;

public class OptionManager : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel;
    private bool isOpen = false;
    public bool OptionVisible => isOpen;

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
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void CloseOptions()
    {
        if (isOpen) SetOptions(false);
    }
}
