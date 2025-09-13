using System.Collections;
using UnityEngine;

public class MeleeAttackSword : MonoBehaviour
{
    Animator animator;
    bool canAttackWeak = true;
    bool canAttackStrong = true;
    [SerializeField] Transform shotPoint;
    [SerializeField] GameObject attackVFX;
    [SerializeField] GameObject attack_2_VFX;
    [SerializeField] Vector2 baseLocalOffset = new Vector2(1f, 0f);
    public bool isUsingSword;
    PlayerMovement playerMovement;
    Transform player;
    public enum lockShooting { none, strong, weak }
    public lockShooting isLocked = lockShooting.none;

    private void Awake()
    {
        player = transform;
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void Attack_1()
    {  
        if (!canAttackWeak || !isUsingSword)
            return;
        if (isLocked != lockShooting.none || !canAttackWeak)
            return;
        animator.SetTrigger("usingLightAttack");
        // instantly face the opposite way
        bool facingRight = playerMovement.isFacingRight;
        shotPoint.right = facingRight ? Vector2.right : Vector2.left;
        float sign = facingRight ? 1 : -1;
        Vector3 local = new Vector3(baseLocalOffset.x * sign, baseLocalOffset.y, 0f);
        // means: “take the point local, which is expressed in the player’s local space,
        // and convert it into a world-space position, then put shotPoint there.”
        shotPoint.position = player.TransformPoint(local);
        Instantiate(attackVFX, shotPoint.position, shotPoint.rotation);
        StartCoroutine(WeakCooldown());
    }
    public void Attack_2()
    {
        if (!canAttackStrong || !isUsingSword)
            return;
        if (isLocked != lockShooting.none || !canAttackStrong)
            return;
        animator.SetTrigger("usingHeavyAttack");
        bool facingRight = playerMovement.isFacingRight;
        shotPoint.right = facingRight ? Vector2.right : Vector2.left;
        float sign = facingRight ? 1 : -1;
        Vector3 local = new Vector3(baseLocalOffset.x * sign, baseLocalOffset.y, 0f);
        // means: “take the point local, which is expressed in the player’s local space,
        // and convert it into a world-space position, then put shotPoint there.”
        shotPoint.position = player.TransformPoint(local);
        Instantiate(attack_2_VFX, shotPoint.position, shotPoint.rotation);
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
