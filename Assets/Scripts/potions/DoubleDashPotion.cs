using System.Collections;
using UnityEngine;

public class DoubleDashPotion : MonoBehaviour
{
    private PlayerMovement playerMovement;
    [SerializeField] float timeInBetween;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerMovement = collision.GetComponent<PlayerMovement>();
            Debug.Log("the player collides with the potion");
            if (playerMovement != null)
            {
                playerMovement.dashCooldown = timeInBetween;
            }
            Destroy(gameObject);
        }
    }
}
