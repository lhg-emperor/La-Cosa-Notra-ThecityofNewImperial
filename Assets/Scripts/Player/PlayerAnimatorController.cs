using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetWeaponType(string weaponName)
    {
        int id = weaponName switch
        {
            "Bat" => 1,
            "Katana" => 2,
            _ => 0
        };
        animator.SetInteger("weaponTypeID", id);
    }

    public void OnRunning(bool isRunning)
    {
        animator.SetBool("isRunning", isRunning);
    }

    public void Attack()
    {
        animator.SetBool("isHit", true);
    }

    public void ResetAttack()
    {
        animator.SetBool("isHit", false);
    }
}
