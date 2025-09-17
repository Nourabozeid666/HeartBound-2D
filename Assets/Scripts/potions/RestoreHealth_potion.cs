using UnityEngine;

public class RestoreHealth_potion : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var healthState = collision.GetComponent<PlayerState>();
            healthState.currentHealth = 50;
            var healthBarState = FindFirstObjectByType<HealthBar>();
            healthBarState.RestoreHealthBar();
            Destroy(gameObject);
        }
    }
}
