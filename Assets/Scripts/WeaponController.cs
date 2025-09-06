using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] GameObject gun;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        { 
            Destroy(this.gameObject);
            gun.SetActive(true) ;
        }
    }
}
