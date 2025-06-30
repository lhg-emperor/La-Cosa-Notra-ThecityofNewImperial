using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PressAnyKey : MonoBehaviour
{
    public GameObject pressAnyKeyText;
    public GameObject screenPanel;
    public GameObject escToQuitText;

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
        if (keyPressed && Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(); // Thoát game (sẽ không hoạt động trong Editor)
            Debug.Log("Quit Game"); // Dùng để kiểm tra khi chạy trong Editor
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Dừng Playmode khi trong Editor
#else
    Application.Quit(); // Thoát game thật khi build
#endif
        }
    }
}
