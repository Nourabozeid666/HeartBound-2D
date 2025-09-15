using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletDestroyingTime;
    [SerializeField] float steamTime;
    [SerializeField] float speed;
    [SerializeField] float steamSpeed;
    private Animator animator;
    [SerializeField] float BulletDamage;
    // was BulletDamage
    // per-instance setter/getter
    public float damage
    {
        get => BulletDamage;
        set => BulletDamage = value;
    }
    bool undestroyedBullet=true;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        StartCoroutine(bulletSystem());
       // rb.linearVelocity = Vector2.up * speed;
    }
    IEnumerator bulletSystem()
    {
        yield return new WaitForSeconds(bulletDestroyingTime);
        if(gameObject.CompareTag("Bullet"))
        animator.SetBool("active", true);
        undestroyedBullet = false;
        yield return new WaitForSeconds(steamTime);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (undestroyedBullet)
            transform.Translate(Vector3.right * speed * Time.deltaTime, Space.Self);
        else
            transform.Translate(Vector3.right * steamSpeed * Time.deltaTime, Space.Self);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")&&undestroyedBullet)
        {
            EnemyHealth hitEnemy = collision.GetComponent<EnemyHealth>();
            if (hitEnemy != null)
            {
                hitEnemy.TakeDamage(BulletDamage);
            }
            Destroy(gameObject);
        }
    }
    }
