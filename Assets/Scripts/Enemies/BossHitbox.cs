using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitbox : MonoBehaviour
{
    [SerializeField] Transform hitPoint;
    [SerializeField] float radius = 1.0f;
    [SerializeField] LayerMask playerMask;
    [SerializeField] int damage = 12;

    [SerializeField] bool damageOncePerActivation = true;

    [SerializeField] float tickInterval = 0.12f;

    HashSet<PlayerState> _alreadyHit = new HashSet<PlayerState>();

    public void Activate(float activeTime)
    {
        if (!gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        _alreadyHit.Clear();
        if (damageOncePerActivation)
            StartCoroutine(ActiveWindow_Once(activeTime));
        else
            StartCoroutine(ActiveWindow_Ticks(activeTime, tickInterval));
    }

    IEnumerator ActiveWindow_Once(float t)
    {
        float end = Time.time + t;

        // ??? ???? ???? ???? ???????? ????? ??????
        while (Time.time < end)
        {
            TryHitOnce();
            yield return null;
        }
    }
    IEnumerator ActiveWindow_Ticks(float t, float interval)
    {
        float end = Time.time + t;
        float next = Time.time;
        while (Time.time < end)
        {
            if (Time.time >= next)
            {
                TryHitOnce();
                next = Time.time + interval;
            }
            yield return null;
        }
    }

    void TryHitOnce()
    {
        Vector2 c = hitPoint ? (Vector2)hitPoint.position : (Vector2)transform.position;
        var hit = Physics2D.OverlapCircle(c, radius, playerMask);
        if (!hit) return;

        var ps = hit.GetComponentInParent<PlayerState>();
        if (!ps) return;

        if (_alreadyHit.Contains(ps)) return; 
        ps.TakeDamage(damage);
        _alreadyHit.Add(ps);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Vector3 c = hitPoint ? hitPoint.position : transform.position;
        Gizmos.DrawWireSphere(c, radius);
    }
}
