using UnityEngine;
public class SwordController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            var playerAnim = collision.GetComponentInParent<Animator>();
            var playerMelee = playerAnim.GetComponentInParent<MeleeAttackSword>();
            if (playerAnim != null)
            {
                playerAnim.SetBool("isHoldingSword", true);
                Destroy(gameObject);
            }
            if (playerMelee != null)
            {
                playerMelee.isUsingSword = true;
            }
        }
    }
}
