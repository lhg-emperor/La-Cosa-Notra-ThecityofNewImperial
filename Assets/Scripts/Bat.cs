using UnityEngine;
using UnityEngine.InputSystem;

public class Bat : MonoBehaviour
{
    public int batDamage = 10;
    public bool canPickUp = false;
    public bool canDrop = false;

    public GameObject Player;

    public PlayerControls playerControls;
    public InputAction pickupAction;



    public void Awake()
    {
        playerControls = new PlayerControls();
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
        if(canPickUp && pickupAction.WasPressedThisFrame())
        {
            Debug.Log(message: "Đã lụm");
            gameObject.SetActive(false);
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            canPickUp = true;
            Debug.Log(message:"");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            canPickUp = false;
            Debug.Log(message:"hehe");
        }
    }
}
