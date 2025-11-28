using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGameButton : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button loadButton;

    void Start()
    {
        if (loadButton != null)
        {
            loadButton.onClick.AddListener(OnLoadGameClicked);

            // Vô hiệu hóa nút nếu chưa có dữ liệu
            if (DataPersistenceManager.instance == null || !DataPersistenceManager.instance.HasGameData())
            {
                loadButton.interactable = false;
                Debug.Log("Không tìm thấy dữ liệu lưu, vô hiệu hóa nút Load Game.");
            }
        }
        else
        {
            Debug.LogWarning("Load Button chưa được gán!");
        }
    }

    private void OnLoadGameClicked()
    {
        var manager = DataPersistenceManager.instance;
        if (manager == null)
        {
            Debug.LogWarning("Không tìm thấy DataPersistenceManager!");
            return;
        }

        // Kiểm tra xem có dữ liệu lưu thật sự không
        if (!manager.HasGameData())
        {
            Debug.Log("Không có dữ liệu lưu để Load Game.");
            return;
        }

        // Indicate that this load was explicitly requested by the player
        manager.RequestedLoad = true;
        // Load dữ liệu
        manager.LoadGame();

        // Lấy tên Scene từ dữ liệu đã lưu
        string sceneToLoad = manager.CurrentGameData != null
            ? manager.CurrentGameData.currentSceneName
            : null;

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("💾 Load Game -> Scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Dữ liệu lưu không có thông tin Scene, không Load Game.");
        }
    }
}
