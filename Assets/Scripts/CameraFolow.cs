using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Biến tham chiếu tới Main Player
    public Transform target;

    // Khoảng cách camera so với Player (có thể chỉnh trong Inspector)
    public Vector3 offset = new Vector3(0, 0, -10);

    // Cập nhật vị trí camera sau khi Player di chuyển
    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: Chưa gán target Player!");
            return;
        }

        // Camera luôn giữ vị trí bằng vị trí Player + offset
        transform.position = target.position + offset;
    }
}
