using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // Nhân vật chính

    public Vector3 offset = new Vector3(0, 10, -10); // Khoảng cách camera so với nhân vật
    public float smoothSpeed = 0.125f;              // Độ mượt khi theo

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;

        transform.rotation = Quaternion.Euler(45, 0, 0); // giữ camera góc nhìn cố định (ví dụ 45 độ nhìn xuống)
    }
}
