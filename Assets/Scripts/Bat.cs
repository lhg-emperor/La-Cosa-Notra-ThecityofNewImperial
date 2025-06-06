using UnityEngine;
using UnityEngine.InputSystem;

public class Bat : MonoBehaviour
{
    public int batDamage = 10;
    public bool canPickUp = false;

    private GameObject Player;

    public PlayerControls playerControls;
    public InputAction pickupAction;

    public void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();
        pickupAction = playerControls.PickUp.pick;
    }

    void OnEnable()
    {
        pickupAction.Enable();

    }

    private void OnDisable()
    {
        pickupAction.Disable();
    }

    private void Update()
    {
        if (canPickUp && pickupAction.WasPressedThisFrame())
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canPickUp = true;
            Player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canPickUp = false;
        }
    }
}
