using System.Collections;
using UnityEngine;

public class MeleeAttackSword : MonoBehaviour
{
    Animator animator;
    bool canAttackWeak = true;
    bool canAttackStrong = true;
    enum lockShooting { none, strong, weak }
    lockShooting isLocked = lockShooting.none;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void Attack_1()
    {
        if (!canAttackWeak)
            return;
        if (isLocked != lockShooting.none || !canAttackWeak)
            return;
        animator.SetTrigger("usingLightAttack");
        StartCoroutine(WeakCooldown());
    }
    public void Attack_2()
    {
        if (!canAttackStrong)
            return;
        if (isLocked != lockShooting.none || !canAttackStrong)
            return;
        animator.SetTrigger("usingHeavyAttack");
        StartCoroutine(StrongCooldown());

    }
    IEnumerator WeakCooldown()
    {
        isLocked = lockShooting.weak;
        canAttackWeak = false;
        yield return new WaitForSeconds(0.45f);
        canAttackWeak = true;
        isLocked = lockShooting.none;
    }
    IEnumerator StrongCooldown()
    {
        isLocked = lockShooting.strong;
        canAttackStrong = false;
        yield return new WaitForSeconds(0.7f);
        canAttackStrong = true;
        isLocked = lockShooting.none;
    }
}
