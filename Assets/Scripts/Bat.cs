using UnityEngine;
using UnityEngine.InputSystem;

public class Bat : MonoBehaviour
{
    public int damage = 15;
    public bool canPickUp = false;

    private GameObject Player;

    public PlayerControls playerControls;
    public InputAction pickupAction;
    public InputAction dropAction;

    public void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();
    }

    void OnEnable()
    {
        pickupAction.Enable();
        dropAction.Enable();
        if (playerControls != null)
            playerControls.Disable();
    }

    private void OnDisable()
    {
        pickupAction.Disable();
        dropAction.Disable();
    }

    private void Update()
    {
        if (canPickUp && pickupAction.WasPressedThisFrame())
        {
            Destroy(gameObject);
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
