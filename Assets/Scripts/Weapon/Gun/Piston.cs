using UnityEngine;

public class Piston : MonoBehaviour, IGun
{
    [Header("Piston Setting")]
    public float PisDamage = 21.5f;
    public int ammo = 30;
    public bool canPickUp = false;

    public RuntimeAnimatorController pistonAnimator;
    public GameObject bulletPre;
    public Transform firePoint;

    public bool CanPickUp => canPickUp;
    public float GetDamage() => PisDamage;

    public void OnPickUp()
    {
        gameObject.SetActive(false);
    }

    public void OnDrop(Vector3 dropPosition)
    {
        transform.position = dropPosition;
        gameObject.SetActive(true);
    }

    public RuntimeAnimatorController GetAnimatorController()
    {
        return pistonAnimator;
    }

    public void Fire(playerPickup owner)
    {
        if (ammo <= 0) return;
        if (owner.IsAttacking) return;

        owner.IsAttacking = true;

        owner.animator.SetBool("isShoot", true);
        owner.StartCoroutine(FireAfterAnimation(owner));
    }

    private System.Collections.IEnumerator FireAfterAnimation(playerPickup owner)
    {
        yield return new WaitForSeconds(0.48f);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - owner.transform.position).normalized;

        GameObject bullet = Object.Instantiate(bulletPre, firePoint.position, Quaternion.identity);
        PistonBullet bulletScript = bullet.GetComponent<PistonBullet>();
        bulletScript.Initialize(dir, PisDamage, owner.GetComponent<Collider2D>());

        // GỌI WantedSystem
        if (owner.CompareTag("Player"))
        {
            if (WantedSystem.Instance.GetWantedLevel() < 1)
            {
                WantedSystem.Instance.SetWantedLevel(1);
                Debug.Log("[WantedSystem] Player nổ súng → Truy nã 1 sao");
            }
        }

        ammo--;
        owner.animator.SetBool("isShoot", false);
        owner.IsAttacking = false;

        if (ammo <= 0)
        {
            owner.currentGun = null;
            owner.CurrentDamage = owner.baseDamage;

            owner.animator.runtimeAnimatorController = owner.defaultAnimator;

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($" Piston OnTriggerEnter2D với {collision.name}");
            canPickUp = true;
            collision.GetComponent<playerPickup>()?.SetNearWeapon(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canPickUp = false;
            collision.GetComponent<playerPickup>()?.ClearNearWeapon(gameObject);
        }
    }
}
