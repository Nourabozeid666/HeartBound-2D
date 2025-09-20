using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int damage = 10;

    [Header("Lifetime")]
    [SerializeField] float lifeTime = 5f;

    Vector2 dir;
    float speed;
    bool hasRB;

    public void SetDamage(int d) => damage = d;

    void Awake()
    {
        hasRB = TryGetComponent<Rigidbody2D>(out _);
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var ps = other.GetComponent<PlayerState>();
        if (ps)
        {
            ps.TakeDamage(damage);
            Destroy(gameObject);
        }

    }

    public void Init(Vector2 direction, float spd)
    {
        dir = direction.normalized;
        speed = spd;
    }

    void Update()
    {
        if (hasRB) return;                
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }
}
