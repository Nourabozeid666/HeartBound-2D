using UnityEngine;
public class SwordController : MonoBehaviour
{
    [SerializeField] Animator playerAnim;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            var anim = collision.GetComponentInParent<Animator>();
            if (anim != null)
            {
                anim.SetBool("isHoldingSword", true);
                Destroy(gameObject);
            }
        }
    }
}
