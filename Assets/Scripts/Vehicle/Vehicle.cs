using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VehicleController : MonoBehaviour
{
    [Header("Settings")]
    public float maxSpeed = 10f;       // Vmax
    public float timeToMax = 5f;       // Thời gian để đạt max speed
    public float turnSpeed = 200f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isDriving = false;

    private float currentSpeed = 0f;
    private float acceleration;        // a = Vmax / T
    private int accelDir = 0;
    private int prevInputDir = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        acceleration = maxSpeed / timeToMax;
    }

    public void EnableDriving(bool enable)
    {
        isDriving = enable;
        moveInput = Vector2.zero;
        currentSpeed = 0f;
        accelDir = 0;
        prevInputDir = 0;
        rb.linearVelocity = Vector2.zero;
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    void FixedUpdate()
    {
        if (!isDriving) return;

        float inputY = moveInput.y;
        float dt = Time.fixedDeltaTime;
        int inputDir = 0;
        if (Mathf.Abs(inputY) > 0.01f) inputDir = (inputY > 0) ? 1 : -1;

        // ================== Nhấn input ==================
        if (inputDir != 0)
        {
            // Nếu đổi hướng → reset tốc độ
            if (inputDir != prevInputDir && prevInputDir != 0)
            {
                // Có thể để nguyên currentSpeed, hoặc triệt tiêu nhanh hơn
                // ở đây ta giữ nguyên để phản ánh quán tính
            }

            accelDir = inputDir;
            currentSpeed += accelDir * acceleration * dt;
        }
        else
        {
            // ================== Nhả input ==================
            if (Mathf.Abs(currentSpeed) > 0.01f)
            {
                int decelDir = (currentSpeed > 0) ? -1 : 1;
                currentSpeed += decelDir * acceleration * dt;

                if (Mathf.Abs(currentSpeed) < 0.05f)
                    currentSpeed = 0f;
            }
            else
            {
                accelDir = 0;
            }
        }

        // Clamp tốc độ
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Di chuyển xe
        Vector2 forward = transform.up * currentSpeed * dt;
        rb.MovePosition(rb.position + forward);

        // Quay xe
        float rotation = -moveInput.x * turnSpeed * dt;
        rb.MoveRotation(rb.rotation + rotation);

        prevInputDir = inputDir;
    }
}
