using UnityEngine;

public class Faster_Special_gun : MonoBehaviour
{
    private Gun gun;
    [SerializeField] float strongShotsTime;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("the player is colliding with the bottle");
            gun = collision.GetComponent<Gun>();
            if (gun != null)
            {
                gun.strongShotsTime = strongShotsTime;
            }
            Destroy(gameObject);
        }
    }
}
