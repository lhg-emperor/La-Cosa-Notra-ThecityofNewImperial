using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VehiclePhysics : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private float DriftFactor = 0.5f;         // Giảm drift
    [SerializeField] private float Acceleration = 30f;         // Gia tốc
    [SerializeField] private float TurnFactor = 3.5f;          // Tốc độ xoay
    [SerializeField] private float BrakeFactor = 0.1f;         // Chỉ số hãm khi ngược chiều
    [SerializeField] private float MaxSpeed = 20f;             // Vận tốc tối đa

    private float AInput = 0f;          // Input tiến/lùi
    private float SteeringInput = 0f;   // Input rẽ trái/phải

    private float rotationAngle = 0f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ResetPhysics()
    {
        rb.linearVelocity = Vector2.zero;
        rotationAngle = 0f;
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
        // Hướng tiến
        Vector2 forward = transform.up;

        // Lấy tốc độ hiện tại theo hướng tiến
        float forwardSpeed = Vector2.Dot(rb.linearVelocity, forward);

        // Hãm khi nhấn ngược chiều
        if (forwardSpeed * AInput < 0)
        {
            // Khi đang tiến mà nhấn lùi, giảm tốc
            rb.linearVelocity *= (1f - BrakeFactor);
        }

        // Tăng tốc theo input
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
