using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletDestroyingTime;
    [SerializeField] float speed;
    private Animator animator;
    [SerializeField] float BulletDamage;
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
        yield return new WaitForSeconds(1f);
        animator.SetBool("active", true);
        undestroyedBullet = false;
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime, Space.Self);
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
