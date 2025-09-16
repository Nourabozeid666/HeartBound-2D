using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState : MonoBehaviour
{
    //any damage suppose to be = 5
    public int maxHealth = 50;
    public int lives;
    public int currentHealth;
    [SerializeField] Animator animator;
    [SerializeField] PlayerMovement playerMovement;
     HealthBar healthBar;
    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }
    public void TakeDamage(int damage)
    {
        if(!playerMovement.isDashing)
        {
            healthBar = FindFirstObjectByType<HealthBar>();
            animator.SetTrigger("isHurt");
            currentHealth -= damage;
            if (currentHealth < 0)
                currentHealth = 0;
            if (lives > 0 && currentHealth == 0)
            {
                currentHealth = maxHealth;
            }
            if (lives == 0 && currentHealth == 0)
            {
                StartCoroutine(HandleDeath());
            }
            else
            {
                animator.SetTrigger("isHurt");
                //AudioManager.instance.RandomizeSfx(hurt);
            }
            if (healthBar.targetIndex !=30)
                healthBar.targetIndex += 3;
            Debug.Log("the Player lives : " + lives);
            Debug.Log("the Player health : " + currentHealth);
        }
    }
    IEnumerator HandleDeath()
    {

        animator.SetBool("isDead", true);
        yield return null;
        yield return new WaitForSeconds(1.2f);
        SceneController.instance.GameOver();
    }

}
