using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VehicleController : MonoBehaviour
{
    [Header("Settings")]
    public float maxSpeed = 100f;       // Vmax (Đặt 100 cho ví dụ)
    public float timeToMax = 5f;        
    public float turnSpeed = 200f;
    public float maxAcceleration = 10f; // Amax (Đặt 10 cho ví dụ)
    public float maxDeceleration = 15f; // Gia tốc phanh tối đa khi nhả Input (>= Amax)
    public float minDecelFactor = 0.2f; // Gia tốc phanh tối thiểu (20% Amax)

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isDriving = false;

    private float currentSpeed = 0f;
    private int prevInputDir = 0; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (maxAcceleration <= 0.01f)
        {
            maxAcceleration = maxSpeed / timeToMax;
        }
    }

    public void EnableDriving(bool enable)
    {
        isDriving = enable;
        moveInput = Vector2.zero;
        currentSpeed = 0f;
        prevInputDir = 0;
        rb.linearVelocity = Vector2.zero;
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private float CalculateDrivingAcceleration(float currentAbsSpeed)
    {
        if (maxSpeed <= 0.01f) return maxAcceleration; 
        
        return maxAcceleration * (1f - Mathf.Clamp01(currentAbsSpeed / maxSpeed));
    }
    
    private float CalculateIdleDeceleration(float currentAbsSpeed)
    {
        float idleBrakingA = maxDeceleration * Mathf.Clamp01(currentAbsSpeed / maxSpeed);
        
        return Mathf.Max(idleBrakingA, maxAcceleration * minDecelFactor); 
    }

    void FixedUpdate()
    {
        if (!isDriving) return;

        float inputY = moveInput.y;
        float dt = Time.fixedDeltaTime;
        
        int inputDir = 0;
        if (Mathf.Abs(inputY) > 0.01f) inputDir = (inputY > 0) ? 1 : -1;

        int currentMoveDir = (currentSpeed > 0.01f) ? 1 : ((currentSpeed < -0.01f) ? -1 : 0);
        
        float accelerationMagnitude = 0f;
        float absSpeed = Mathf.Abs(currentSpeed);
        
        if (inputDir != 0)
        {
            currentSpeed += accelerationMagnitude * inputDir * dt;
        }
        else
        {
            if (absSpeed > 0.01f)
            {
                int frictionDir = (currentSpeed > 0) ? -1 : 1; 
                
                accelerationMagnitude = CalculateIdleDeceleration(absSpeed); 
                
                currentSpeed += accelerationMagnitude * frictionDir * dt;

                if (Mathf.Abs(currentSpeed) < 0.05f)
                    currentSpeed = 0f;
            }
        }

        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        Vector2 forward = transform.up;

        float rotation = -moveInput.x * turnSpeed * dt;
        rb.MoveRotation(rb.rotation + rotation);

        prevInputDir = inputDir;
    }
}
