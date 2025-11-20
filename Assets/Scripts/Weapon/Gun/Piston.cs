using UnityEngine;
using System.Collections;

public class Piston : MonoBehaviour, IGun
{
    [Header("Piston Settings")]
    public float PisDamage = 21.5f;
    public int ammo = 30;
    public int totalReserveAmmo = 30;

    public bool canPickUp = false;

    public RuntimeAnimatorController pistonAnimator;
    public GameObject bulletPre;

    [Header("UI & Logic")]
    public Sprite iconSprite;
    private UIManager uiManager;

    private readonly Vector3 bulletOffset = new Vector3(0.023f, 0.224f, 0f);

    // ==== Thuộc tính cơ bản ====
    public bool CanPickUp => canPickUp;
    public float GetDamage() => PisDamage;
    public Sprite WeaponIcon => iconSprite;
    public int CurrentMagazineAmmo => ammo;
    public int TotalReserveAmmo => totalReserveAmmo;

    public void SetUIManager(UIManager manager) => uiManager = manager;

    public void OnPickUp() => gameObject.SetActive(false);

    public void OnDrop(Vector3 dropPosition)
    {
        transform.position = dropPosition;
        gameObject.SetActive(true);
    }

    public RuntimeAnimatorController GetAnimatorController() => pistonAnimator;

    // ==== Bắn đạn ====
    public void Fire(playerPickup owner)
    {
        if (ammo <= 0 || owner.IsAttacking) return;

        owner.IsAttacking = true;
        owner.animator.SetBool("isShoot", true);
        owner.StartCoroutine(FireAfterAnimation(owner));
    }

    private IEnumerator FireAfterAnimation(playerPickup owner)
    {
        yield return new WaitForSeconds(0.48f);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - owner.transform.position).normalized;

        // Tạo đạn tại vị trí tương đối so với Player
        Vector3 spawnPos = owner.transform.position + bulletOffset;
        GameObject bullet = Instantiate(bulletPre, spawnPos, Quaternion.identity);
        bullet.GetComponent<PistonBullet>()?.Initialize(dir, PisDamage, owner.GetComponent<Collider2D>());

        ammo--;
        uiManager?.UpdateWeaponDisplay(WeaponIcon, CurrentMagazineAmmo, TotalReserveAmmo);

        owner.animator.SetBool("isShoot", false);
        owner.IsAttacking = false;

        if (ammo <= 0)
        {
            owner.currentGun = null;
            owner.CurrentDamage = owner.baseDamage;
            owner.animator.runtimeAnimatorController = owner.defaultAnimator;

            if (uiManager?.defaultIcon != null)
                uiManager.UpdateWeaponDisplay(uiManager.defaultIcon, 0, 0);

            Destroy(gameObject);
        }
    }

    // ==== Trigger nhặt vũ khí ====
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        canPickUp = true;
        collision.GetComponent<playerPickup>()?.SetNearWeapon(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        canPickUp = false;
        collision.GetComponent<playerPickup>()?.ClearNearWeapon(gameObject);
    }
}
