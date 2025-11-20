using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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

    public void SetIsShoot(bool isShoot)
    {
        animator.SetBool("isShoot", isShoot);
    }
    private bool HasParameter(string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
}
