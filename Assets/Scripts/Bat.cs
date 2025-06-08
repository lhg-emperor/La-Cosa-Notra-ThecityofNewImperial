using UnityEngine;
using UnityEngine.InputSystem;

public class Bat : MonoBehaviour
{
    public int batDamage = 15;
    public bool canPickUp = false;

    private GameObject Player;

    public PlayerControls playerControls;
    public InputAction pickupAction;
    public InputAction dropAction;

    public void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();
        pickupAction = playerControls.PickUp.pick;
        dropAction = playerControls.PutDown.drop;
    }

    void OnEnable()
    {
        pickupAction.Enable();
        dropAction.Enable();
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
            gameObject.SetActive(false);
        }
        if (!gameObject.activeSelf && dropAction.WasPressedThisFrame())
        {
            gameObject.SetActive(true);
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
