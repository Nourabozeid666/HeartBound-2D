using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int damage = 10;
    [SerializeField] float lifeTime = 5f;

    public void SetDamage(int d) => damage = d;

    void Awake() => Destroy(gameObject, lifeTime);

    void OnTriggerEnter2D(Collider2D other)
    {
        var ps = other.GetComponent<PlayerState>();
        if (ps) { ps.TakeDamage(damage); Destroy(gameObject); }
        else { Destroy(gameObject); }
    }
}
