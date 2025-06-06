using UnityEditor.Tilemaps;
using UnityEngine;

public class Citizen : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Rigidbody2D rb;
    private Vector2 moveCitizen;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveCitizen * (moveSpeed * Time.fixedDeltaTime));
    }
    public void Moveto(Vector2 Targetposition)
    {
        moveCitizen = Targetposition;
    }
}
