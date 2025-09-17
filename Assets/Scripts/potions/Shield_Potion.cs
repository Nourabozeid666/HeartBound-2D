using UnityEngine;
using UnityEngine.InputSystem;

public class Shield_Potion : MonoBehaviour
{
    Shield Shield;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Shield.ShieldIsActive = true;
            var ShieldBarUI = FindFirstObjectByType<ShoeldBarUI>(FindObjectsInactive.Include);
            if (ShieldBarUI != null)
            {
                Shield.enabled = true;
            }
            Destroy(this.gameObject);
        }
    }
}
