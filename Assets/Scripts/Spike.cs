using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AudioClip lava;
    [SerializeField] int damage;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (animator && gameObject.tag == "BearTrap")
            {
                animator.SetTrigger("works");
                Destroy(gameObject);
            }
            FindFirstObjectByType<PlayerState>().TakeDamage(damage);
        }

    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.tag == "Lava")
        {
            FindFirstObjectByType<PlayerState>().TakeDamage(damage);
        }
    }
}
