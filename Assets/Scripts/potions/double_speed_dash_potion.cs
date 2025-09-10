using UnityEngine;

public class double_speed_dash_potion : MonoBehaviour
{
    private PlayerMovement playerMovement;
    [SerializeField] float dashSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerMovement = collision.GetComponent<PlayerMovement>();
            Debug.Log("the player collides with the potion");
            if (playerMovement != null)
            {
                playerMovement.dashSpeed = dashSpeed;
            }
            Destroy(gameObject);
        }
    }
}
