using UnityEngine;
using UnityEngine.InputSystem;

public class Shield_Potion : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var ui = GameObject.FindGameObjectWithTag("ui");
            var shield = ui ? ui.GetComponentInChildren<Shield>(true) : null;
            shield.ShieldIsActive = true;
            FindFirstObjectByType<ShoeldBarUI>(FindObjectsInactive.Include)?.gameObject.SetActive(true); 
            Destroy(this.gameObject);
        }
    }
}
