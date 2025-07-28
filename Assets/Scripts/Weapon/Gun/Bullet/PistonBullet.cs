using UnityEngine;

public class PistonBullet : MonoBehaviour
{
    public float speed = 50f;
    public float damage = 10f;

    public Rigidbody2D rb;

    private Collider2D ownerCollider;

    public void Initialize(Vector2 direction, float damageAmount, Collider2D owner)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * speed;
        damage = damageAmount;
        ownerCollider = owner;

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), ownerCollider, true);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo == ownerCollider) return;

        if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject);
            return;
        }

        IDamageable target = hitInfo.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage, transform);
            Destroy(gameObject);
        }
    }
}
