using UnityEngine;

// Qu?n lý hi?n th? menu Option (ESC ?? m?/?óng) và ch?c n?ng Pause
public class OptionManager : MonoBehaviour
{
    [Header("Panel Option (gán trong Inspector)")]
    [SerializeField] private GameObject optionPanel;

    private bool isOpen = false;

    // Cho phép các script khác ??c tr?ng thái
    public bool OptionVisible => isOpen;

    void Awake()
    {
        if (optionPanel != null)
            optionPanel.SetActive(false);
        // ??m b?o game không b? paused khi kh?i t?o
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Dùng phím ESC ?? toggle menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptions();
        }
    }

    // G?i hàm này ?? m?/?óng menu t? code ho?c UI Button
    public void ToggleOptions()
    {
        SetOptions(!isOpen);
    }

    // Thi?t l?p tr?ng thái hi?n th? menu và pause
    public void SetOptions(bool show)
    {
        if (optionPanel == null)
        {
            Debug.LogWarning("OptionManager: optionPanel ch?a ???c gán trong Inspector.");
            return;
        }

        isOpen = show;
        optionPanel.SetActive(isOpen);

        if (isOpen)
        {
            // Pause game và hi?n th? con tr?
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Resume game và ?n con tr?
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Hàm ti?n ích ?? ?óng menu (ví d? g?n vào nút Close)
    public void CloseOptions()
    {
        if (isOpen) SetOptions(false);
    }
}
