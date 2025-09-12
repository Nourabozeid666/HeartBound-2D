using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float health;
    float currentHealth;
    public bool isDead = false;
    [SerializeField] Animator animator;
    [SerializeField] AudioClip enemyHurt;
    [SerializeField] AudioClip enemyDead;
    //private bool recentlyHit = false;
    void Start()
    {
        currentHealth = health;
    }
    public void TakeDamage(float damage)
    {
        if (isDead ) return;
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;
        if (currentHealth == 0)
        {
            //AudioManager.instance.EnemyAction(enemyDead);
            animator.SetBool("isDead", true);
            isDead = true;
            StartCoroutine(DeathCleanup());
        }
        else
        {
            //AudioManager.instance.EnemyAction(enemyHurt);
            HitReactionRoutine();
        }
    }
    //to avoid the double hit from the back
   // IEnumerator HitCooldown()
   // {
   //     recentlyHit = true;
   //     yield return new WaitForSeconds(0.4f); // adjust based on animation/delay
   //     recentlyHit = false;
   // }
    void HitReactionRoutine()
    {
        StartCoroutine(EnemyHitReaction());
    }
    IEnumerator EnemyHitReaction()
    {
        animator.SetBool("isHurt", true);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        animator.SetBool("isHurt", false);
    }
    IEnumerator DeathCleanup()
    {
        yield return new WaitForSeconds(1f);
        Destroy(transform.parent.gameObject);

    }
}
