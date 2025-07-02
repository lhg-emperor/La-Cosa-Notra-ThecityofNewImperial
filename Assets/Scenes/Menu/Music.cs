using UnityEngine;

public class PersistentMusic : MonoBehaviour
{
    private static PersistentMusic instance;

    private void Awake()
    {
        // Nếu chưa có instance => giữ lại
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Không bị hủy khi load scene mới
        }
        else
        {
            Destroy(gameObject); // Đã có rồi thì xóa cái mới để tránh trùng nhạc
        }
    }
}
