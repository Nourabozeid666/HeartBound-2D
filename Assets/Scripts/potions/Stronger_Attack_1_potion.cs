using UnityEngine;
using UnityEngine.AI;

public class Stronger_Attack_1_potion : MonoBehaviour
{
    private MeleeAttackSword attackController;
    [SerializeField] float slashStrength;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("the player is colliding with the bottle");
            attackController = collision.GetComponent<MeleeAttackSword>();
            if (attackController != null )
            {
                attackController.weakDamageMul = slashStrength; 
            }
            Destroy(gameObject);
        }
    }
}
