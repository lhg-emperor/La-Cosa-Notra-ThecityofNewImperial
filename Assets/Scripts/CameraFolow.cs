using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Biến tham chiếu tới Main Player
    public Transform target;

    // Khoảng cách camera so với Player (có thể chỉnh trong Inspector)
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Clamping")]
    [Tooltip("Giới hạn nhỏ nhất cho tâm camera trên trục X (world units)")]
    public float minX = -10f;
    [Tooltip("Giới hạn lớn nhất cho tâm camera trên trục X (world units)")]
    public float maxX = 10f;
    [Tooltip("Giới hạn nhỏ nhất cho tâm camera trên trục Y (world units)")]
    public float minY = -10f;
    [Tooltip("Giới hạn lớn nhất cho tâm camera trên trục Y (world units)")]
    public float maxY = 10f;

    // Cập nhật vị trí camera sau khi Player di chuyển
    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: Chưa gán target Player!");
            return;
        }

        // Camera luôn giữ vị trí bằng vị trí Player + offset
        Vector3 desired = target.position + offset;

        // Áp dụng clamp theo min/max X/Y (giữ nguyên z)
        float clampedX = Mathf.Clamp(desired.x, minX, maxX);
        float clampedY = Mathf.Clamp(desired.y, minY, maxY);
        transform.position = new Vector3(clampedX, clampedY, desired.z);
    }
}
