using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VehiclePhysics : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private float DriftFactor = 0.5f;
    [SerializeField] private float Acceleration = 10f; // ĐÃ GIẢM: Từ 30f xuống 10f để đồng bộ với Camera
    [SerializeField] private float TurnFactor = 3.5f;
    [SerializeField] private float BrakeFactor = 0.1f;
    [SerializeField] private float MaxSpeed = 20f;

    // --- THÊM: Thiết lập vật lý bắt buộc ---
    [Header("Rigidbody Setup")]
    [SerializeField] private float LinearDampingValue = 0.5f; // Đã thêm Damping
    [SerializeField] private float AngularDampingValue = 0.5f;

    private float AInput = 0f;
    private float SteeringInput = 0f;

    private float rotationAngle = 0f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // THÊM: Thiết lập Rigidbody bắt buộc để va chạm hoạt động
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Giúp xe di chuyển mượt hơn
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Ngăn ngừa xuyên tường

        // Thiết lập Damping để xe không bị hãm quá nhanh
        rb.linearDamping = LinearDampingValue;
        rb.angularDamping = AngularDampingValue;

        // Đảm bảo trọng lực bằng 0 (vì xe không nên bị rơi)
        rb.gravityScale = 0f;
    }

    public void ResetPhysics()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f; // Đảm bảo vận tốc góc cũng được reset
        rotationAngle = transform.eulerAngles.z; // Lấy góc hiện tại
        AInput = 0f;
        SteeringInput = 0f;
    }

    public void ApplyInput(Vector2 input)
    {
        AInput = input.y;
        SteeringInput = input.x;
    }

    void FixedUpdate()
    {
        ApplyEngine();
        ApplySteering();
        ApplyDrift();
    }

    void ApplyEngine()
    {
        Vector2 forward = transform.up;
        float forwardSpeed = Vector2.Dot(rb.linearVelocity, forward);

        // Hãm khi nhấn ngược chiều
        if (forwardSpeed * AInput < 0)
        {
            rb.linearVelocity *= (1f - BrakeFactor);
        }

        // Tăng tốc theo input
        // Code này tác động trực tiếp vào linearVelocity, nên Body Type Dynamic là BẮT BUỘC
        Vector2 velocityChange = forward * AInput * Acceleration * Time.fixedDeltaTime;
        rb.linearVelocity += velocityChange;

        // Giới hạn tốc độ
        if (rb.linearVelocity.magnitude > MaxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * MaxSpeed;
        }
    }

    void ApplySteering()
    {
        // Xe chỉ rẽ khi có vận tốc
        float speedFactor = rb.linearVelocity.magnitude / MaxSpeed;
        if (speedFactor > 0.01f) // tránh xoay khi đứng yên
        {
            rotationAngle -= SteeringInput * TurnFactor * speedFactor;
            rb.MoveRotation(rotationAngle);
        }
    }

    void ApplyDrift()
    {
        // Loại bỏ thành phần vận tốc ngang để drift nhẹ
        Vector2 forward = transform.up;
        Vector2 right = transform.right;

        Vector2 forwardVelocity = forward * Vector2.Dot(rb.linearVelocity, forward);
        Vector2 rightVelocity = right * Vector2.Dot(rb.linearVelocity, right);

        rb.linearVelocity = forwardVelocity + rightVelocity * DriftFactor;
    }
}