using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VehiclePhysics : MonoBehaviour
{
   
    [Header("Settings")]
    float AccelerationFactor = 30f;
    float turnFactor = 3.5f;

    // Local Varlable
    float AccelerationInput = 0f;
    float SteeringInput = 0f;

    float rotationAngle = 0f;

    Rigidbody2D VehicleBody;
    private void Awake()
    {
        VehicleBody = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        ApplyEngineForce();

        ApplySteering();
    }
    void ApplyEngineForce()
    {
        Vector2 engineForceVector = transform.up * AccelerationInput * AccelerationFactor;

        VehicleBody.AddForce(engineForceVector, ForceMode2D.Force);

    }

    void ApplySteering()
    {
        rotationAngle -= SteeringInput + turnFactor;

        VehicleBody.MoveRotation(rotationAngle);
    }
    public void SettingInputVector(Vector2 InputVector)
    {
        SteeringInput = InputVector.x;
        AccelerationInput *= InputVector.y;

    }

}
