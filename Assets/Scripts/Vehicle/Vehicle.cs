using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VehicleController : MonoBehaviour
{
    VehiclePhysics PhysicsCar;
    private bool canDrive = false;
    private Vector2 moveInput;
    [Header("Settings")]

    Rigidbody2D VehicleBody;
    private void Awake()
    {
        VehiclePhysics physicsCar = GetComponent<VehiclePhysics>();

    }
    private void Update()
    {
        Vector2 inputVector = Vector2.zero;

        inputVector.x = Input.GetAxis("Honrizontal");
        inputVector.y = Input.GetAxis("Vectical");

        PhysicsCar.SettingInputVector(inputVector);
    }
    private void FixedUpdate()
    {
        

        // ApplySteering();

        if (!canDrive) return;

        // Di chuyển xe
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    // 🟢 Bật/tắt chế độ điều khiển xe
    public void EnableDriving(bool enable)
    {
        canDrive = enable;
    }
}
