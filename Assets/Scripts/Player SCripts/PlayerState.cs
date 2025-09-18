using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState : MonoBehaviour
{
    //any damage suppose to be = 5
    public int maxHealth = 50;
    public int shieldHealth;
    public int lives;
    public int currentHealth;
    [SerializeField] Animator animator;
    [SerializeField] PlayerMovement playerMovement;
    HealthBar healthBar;
    Shield shield;
    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        shieldHealth = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        var ShieldBarUI = GameObject.FindGameObjectWithTag("ShieldUI");
        shield = FindFirstObjectByType<Shield>();
        if(!playerMovement.isDashing && shield.ShieldIsActive == false)
        {
            healthBar = FindFirstObjectByType<HealthBar>();
            //animator.SetTrigger("isHurt");
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
            if (healthBar.targetIndex <= 29)
                healthBar.targetIndex += 3;
            Debug.Log("the Player lives : " + lives);
            Debug.Log("the Player health : " + currentHealth);
        }
        else if(!playerMovement.isDashing && shield.ShieldIsActive == true)
        {
            shieldHealth -= damage;
            if (shieldHealth < 0)
                shieldHealth = 0;
            if ( shieldHealth == 0)
            {
                shield.ShieldIsActive = false;
                ShieldBarUI.SetActive(true);
            }
            if (shield.targetIndexShield <= 29)
                shield.targetIndexShield += 3;
            Debug.Log("the shield health : " + shieldHealth);
        }
    }
    IEnumerator HandleDeath()
    {

        animator.SetBool("isDead", true);
        yield return null;
        yield return new WaitForSeconds(1.2f);
        SceneController.instance.GameOver();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V)) 
            TakeDamage(5);
    }

}
