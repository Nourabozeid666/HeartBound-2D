using UnityEngine;

public class longerDistance_dash_potion : MonoBehaviour
{
    private PlayerMovement playerMovement;
    [SerializeField] float dashTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerMovement = collision.GetComponent<PlayerMovement>();
            Debug.Log("the player collides with the potion");
            if (playerMovement != null)
            {
                playerMovement.dashDuration = dashTime;
            }
            Destroy(gameObject);
        }
    }
}
