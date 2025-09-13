using UnityEngine;
using UnityEngine.AI;

public class Stronger_Attack_1_potion : MonoBehaviour
{
    private MeleeAttackSword meleeAttack;
    [SerializeField] float dashSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            meleeAttack = collision.GetComponent<MeleeAttackSword>();
            Debug.Log("the player collides with the potion");
            if (meleeAttack != null)
            {
               // meleeAttack. = dashSpeed;
            }
            Destroy(gameObject);
        }
    }
}
