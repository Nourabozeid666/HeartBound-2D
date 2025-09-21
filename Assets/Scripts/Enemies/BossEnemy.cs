using System.Collections;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Animator anim;         
    [SerializeField] Rigidbody2D rb;
    [SerializeField] EnemyHealth health;    

    [Header("Move")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float chaseStopDist = 1.4f;

    [Header("Ranges")]
    [SerializeField] float meleeRange = 1.8f;
    [SerializeField] float castRangeMin = 2.5f;
    [SerializeField] float castRangeMax = 6.0f;

    [Header("Cooldowns")]
    [SerializeField] float meleeCD = 1.2f;
    [SerializeField] float castCD = 2.0f;

    [Header("Timings")]
    [SerializeField] float meleeWindup = 0.35f, meleeActive = 0.12f, meleeRecover = 0.25f;
    [SerializeField] float castWindup = 0.45f, castRecover = 0.25f;

    [Header("Projectile")]
    [SerializeField] GameObject projectilePrefab;  
    [SerializeField] Transform shootOrigin;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] int projectileDamage = 8;

    [Header("Animator Triggers")]
    [SerializeField] string trigMelee = "DoMelee";
    [SerializeField] string trigCast = "DoCast";

    [Header("Enter Room Delay")]
    [SerializeField] float firstCastDelay = 1.5f;

    float nextMelee, nextCast;
    bool busy;
    Transform player;
    void Awake()
    {
        if (!anim) anim = GetComponentInChildren<Animator>(true);
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!health) health = GetComponent<EnemyHealth>();
    }

    void OnEnable()
    {
        if (health) health.OnDead += HandleDead;
    }
    void OnDisable()
    {
        if (health) health.OnDead -= HandleDead;
    }

    void Start()
    {
        if (!player)
        {
            var p = GameObject.FindWithTag("Player");
            if (p) player = p.transform;
        }

        if (!player)
        {
            player = FindFirstObjectByType<PlayerMovement>().transform;
        }

       
        nextCast = Time.time + firstCastDelay;
    }

    void Update()
    {
        if (!player || health == null) { SetWalk(false); return; }

  
        if (health.IsDead) { StopMovementHard(); return; }

   
        bool stunned = anim && anim.GetBool("isHurt");
        if (busy || stunned) { SetWalk(false); ZeroVelocity(); FacePlayer(); return; }

        float d = Vector2.Distance(transform.position, player.position);

        bool canMelee = d <= meleeRange && Time.time >= nextMelee;
        bool canCast = d >= castRangeMin && d <= castRangeMax && Time.time >= nextCast;

        if (canMelee) { StartCoroutine(DoMelee()); return; }
        if (canCast) { StartCoroutine(DoCast()); return; }

        MoveTowardsPlayer(d);
    }

    // ---------------- Movement ----------------
    void MoveTowardsPlayer(float dist)
    {
        FacePlayer();
        bool shouldMove = dist > chaseStopDist;
        SetWalk(shouldMove);
        if (!shouldMove) { ZeroVelocity(); return; }

        Vector2 dir = (player.position - transform.position).normalized;

        rb.linearVelocity = dir * moveSpeed;

    }

    void FacePlayer()
    {
        var s = transform.localScale;
        s.x = Mathf.Abs(s.x) * (player.position.x >= transform.position.x ? 1 : -1);
        transform.localScale = new Vector2(-s.x,1);
    }
    void SetWalk(bool w) { if (anim) anim.SetBool("isWalking", w); }
    void ZeroVelocity()
    {
        rb.linearVelocity = Vector2.zero;

    }
    void StopMovementHard()
    {
        SetWalk(false);
        ZeroVelocity();
        rb.simulated = false; 
        enabled = false;      
    }

    // ---------------- Attacks ----------------
    IEnumerator DoMelee()
    {
        busy = true; SetWalk(false); ZeroVelocity(); FacePlayer();

        if (anim) { anim.ResetTrigger(trigMelee); anim.SetTrigger(trigMelee); }
        yield return new WaitForSeconds(meleeWindup);

        var hb = GetComponentInChildren<BossHitbox>(true);
        if (hb) hb.Activate(meleeActive);

        yield return new WaitForSeconds(meleeActive + meleeRecover);
        nextMelee = Time.time + meleeCD;
        busy = false;
    }

    IEnumerator DoCast()
    {
        busy = true; SetWalk(false); ZeroVelocity(); FacePlayer();

        if (anim) { anim.ResetTrigger(trigCast); anim.SetTrigger(trigCast); }
        yield return new WaitForSeconds(castWindup);

        if (projectilePrefab && shootOrigin && player)
        {
            Vector2 dir = (player.position - shootOrigin.position).normalized;
            var go = Instantiate(projectilePrefab, shootOrigin.position, Quaternion.identity);

         
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            go.transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);

        
            var proj = go.GetComponent<Projectile>();
            if (proj)
            {
                proj.SetDamage(projectileDamage);
                proj.Init(dir, projectileSpeed);
            }
            var rb2 = go.GetComponent<Rigidbody2D>();

            if (rb2) {
                rb2.gravityScale = 0;
                rb2.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                rb2.linearVelocity = dir * projectileSpeed; 
            }
        }

        yield return new WaitForSeconds(castRecover);
        nextCast = Time.time + castCD;
        busy = false;
    }

    // ---------------- Death hook ----------------
    void HandleDead(EnemyHealth _) { StopMovementHard(); }
}
