using UnityEngine;

public class Stronger_Attack_2_potion : MonoBehaviour
{
    private MeleeAttackSword attackController;
    [SerializeField] float slashStrength;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            attackController = collision.GetComponent<MeleeAttackSword>();
            if (attackController != null)
            {
                attackController.strongDamageMul = slashStrength;
            }
            Destroy(gameObject);
        }
    }
}
