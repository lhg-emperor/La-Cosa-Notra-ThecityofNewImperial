﻿using UnityEngine;

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
}
