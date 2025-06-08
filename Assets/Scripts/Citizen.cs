using UnityEngine;

public class Citizen : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 targetPosition;
    private bool isMoving = false;

    public float Health = 50f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 direction = (targetPosition - rb.position);
            if (direction.magnitude < 0.1f)
            {
                Stop(); 
            }
            else
            {
                moveDirection = direction.normalized;
                rb.MovePosition(rb.position + moveDirection * (moveSpeed * Time.fixedDeltaTime));
            }
        }
    }

    public void Moveto(Vector2 newTarget)
    {
        targetPosition = newTarget;
        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
        moveDirection = Vector2.zero;
    }
}
