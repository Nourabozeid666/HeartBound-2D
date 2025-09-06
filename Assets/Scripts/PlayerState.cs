using System.Collections;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public int health = 50;
    public int lives;
    public int currentHealth;
    [SerializeField] Animator animator;
    [SerializeField] PlayerMovement playerMovement;

    private void Awake()
    {
        currentHealth = health;
        animator = GetComponent<Animator>();
    }
    public void TakeDamage(int damage)
    {

        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;
        if (lives > 0 && currentHealth == 0)
        {
            currentHealth = health;
            //lives--;
           // FindObjectOfType<LevelManager>().RespawnPlayer();
        }
        if (lives == 0 && currentHealth == 0)
        {
            StartCoroutine(HandleDeath());
            Destroy(playerMovement);
        }
        Debug.Log("the Player lives : " + lives);
        Debug.Log("the Player health : " + currentHealth);
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
        
    }
}
