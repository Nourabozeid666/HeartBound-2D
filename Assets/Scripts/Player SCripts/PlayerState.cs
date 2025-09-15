using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState : MonoBehaviour
{
    public int maxHealth = 50;
    public int lives;
    public int currentHealth;
    [SerializeField] Animator animator;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerInput playerInput;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }
    public void TakeDamage(int damage)
    {
        if(!playerMovement.isDashing)
        {
            currentHealth -= damage;
            if (currentHealth < 0)
                currentHealth = 0;
            if (lives > 0 && currentHealth == 0)
            {
                currentHealth = maxHealth;
                //lives--;
                // FindObjectOfType<LevelManager>().RespawnPlayer();
            }
            if (lives == 0 && currentHealth == 0)
            {
                StartCoroutine(HandleDeath());
                Destroy(playerMovement);
                Destroy(playerInput);
            }
            Debug.Log("the Player lives : " + lives);
            Debug.Log("the Player health : " + currentHealth);
        }
    }
    IEnumerator HandleDeath()
    {
        animator.SetBool("isDead", true);
        yield return null;
        yield return new WaitForSeconds(1f);
       // SceneController.instance.ShowGameOver();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        { 
            TakeDamage(2);
            return;
        }
    }
}
