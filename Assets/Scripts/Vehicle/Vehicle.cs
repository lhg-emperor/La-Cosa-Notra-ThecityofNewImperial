using UnityEngine;

[RequireComponent(typeof(VehiclePhysics))]
public class VehicleController : MonoBehaviour
{
    private VehiclePhysics vehiclePhysics;
    private Vector2 moveInput;
    private bool isDriving = false;

    void Awake()
    {
        vehiclePhysics = GetComponent<VehiclePhysics>();
    }

    public void EnableDriving(bool enable)
    {
        isDriving = enable;
        moveInput = Vector2.zero;
        vehiclePhysics.ResetPhysics();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    void FixedUpdate()
    {
        if (!isDriving) return;

        // Chuyển input sang physics để xử lý
        vehiclePhysics.ApplyInput(moveInput);
    }
}
