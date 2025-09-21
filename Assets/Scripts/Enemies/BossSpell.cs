using System.Collections;
using UnityEngine;

public class BossSpell : MonoBehaviour
{
    [SerializeField] float radius = 1.8f;
    [SerializeField] LayerMask playerMask;

    public void Begin(float duration, float tick, int damagePerTick)
    {
        StartCoroutine(Run(duration, tick, damagePerTick));
    }

    IEnumerator Run(float duration, float tick, int dmg)
    {
        float end = Time.time + duration;
        while (Time.time < end)
        {
            var hit = Physics2D.OverlapCircle(transform.position, radius, playerMask);
            if (hit)
            {
                var ps = hit.GetComponent<PlayerState>();
                if (ps) ps.TakeDamage(dmg);
            }
            yield return new WaitForSeconds(tick);
        }
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
