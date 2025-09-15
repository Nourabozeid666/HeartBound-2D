using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int damage = 10;
    [SerializeField] float lifeTime = 5f;

    Vector2 dir;
    float speed;
    public void SetDamage(int d) => damage = d;

    void Awake() => Destroy(gameObject, lifeTime);

    void OnTriggerEnter2D(Collider2D other)
    {
        var ps = other.GetComponent<PlayerState>();
        if (ps) { ps.TakeDamage(damage); Destroy(gameObject); }
        else { Destroy(gameObject); }
    }

    public void Init(Vector2 direction, float spd)
    {
        dir = direction.normalized;
        speed = spd;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }
}