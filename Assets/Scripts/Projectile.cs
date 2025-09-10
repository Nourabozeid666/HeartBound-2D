using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int damage = 10;
    [SerializeField] float lifeTime = 5f;

    public void SetDamage(int d) => damage = d;

    void Awake()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // تجاهلي الأعداء لو لازم باستخدام Layers/Tags (حسب لعبتك)
        var ps = other.GetComponent<PlayerState>();
        if (ps)
        {
            ps.TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            // خبط في أي شيء آخر
            Destroy(gameObject);
        }
    }
}
