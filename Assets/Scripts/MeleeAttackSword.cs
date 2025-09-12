using System.Collections;
using UnityEngine;

public class MeleeAttackSword : MonoBehaviour
{
    Animator animator;
    bool canAttackWeak = true;
    bool canAttackStrong = true;
    [SerializeField] Transform shotPoint;
    [SerializeField] GameObject attackRange;
    public bool isUsingSword;
    enum lockShooting { none, strong, weak }
    lockShooting isLocked = lockShooting.none;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Attack_1()
    {  
        if (!canAttackWeak || !isUsingSword)
            return;
        if (isLocked != lockShooting.none || !canAttackWeak)
            return;
        animator.SetTrigger("usingLightAttack");
        Instantiate(attackRange, shotPoint.position, shotPoint.rotation);
        StartCoroutine(WeakCooldown());
    }
    public void Attack_2()
    {
        if (!canAttackStrong || !isUsingSword)
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
