using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PressAnyKey : MonoBehaviour
{
    public GameObject pressAnyKeyText;
    public GameObject screenPanel;

    private bool keyPressed = false;

    void Start()
    {
        // Ẩn Screen & Buttons khi bắt đầu
        screenPanel.SetActive(false);
    }

    void Update()
    {
        if (!keyPressed && Input.anyKeyDown)
        {
            keyPressed = true;

            // Ẩn dòng chữ "Press any key"
            pressAnyKeyText.SetActive(false);

            // Hiện giao diện chính (Screen + Buttons)
            screenPanel.SetActive(true);
        }
    }
}
