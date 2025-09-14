using UnityEngine;

public class FasterReload : MonoBehaviour
{
    private Gun gun;
    [SerializeField] float relaodSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("the player is colliding with the bottle");
            gun = collision.GetComponent<Gun>();
            if (gun != null)
            {
                gun.reloadtime = relaodSpeed;
            }
            Destroy(gameObject);
        }
    }
}
