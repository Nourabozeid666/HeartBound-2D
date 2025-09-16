using Pathfinding;
using UnityEngine;

public class EnemyChaseLimit : MonoBehaviour
{
    [SerializeField] AIPath aiPath;
    [SerializeField] AIDestinationSetter destSetter;

    [Header("Chase")]
    [SerializeField] float enterChaseRadius = 8f;
    [SerializeField] float exitChaseRadius = 10f;

    [Header("Attack")]
    [SerializeField] float stopDistance = 1.5f;
    [SerializeField] float attackRange = 1.0f;   
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] int Damage = 10;

    [Header("Melee Hitbox (required for valid hit)")]
    [SerializeField] Transform hitPoint;         
    [SerializeField] float hitRadius = 0.6f;  
    [SerializeField] LayerMask playerLayer;     

    [Header("Pathfinding")]
    [SerializeField] float slowdownDistance = 3f;
    [SerializeField] float chaseSpeed = 3.5f;

    enum State { Idle, Chase, Attack }
    State state = State.Idle;

    float lastAttackTime;
    float enterSqr, exitSqr, stopSqr, attackSqr;

    PlayerState playerState;
    Animator Enemyanim;
    Transform player;



    void Start()
    {
       player = FindFirstObjectByType<PlayerMovement>().transform;

    }
    void Awake()
    {
        aiPath = GetComponent<AIPath>();
        destSetter = GetComponent<AIDestinationSetter>();
        Enemyanim = GetComponentInChildren<Animator>(true);

        enterSqr = enterChaseRadius * enterChaseRadius;
        exitSqr = exitChaseRadius * exitChaseRadius;
        stopSqr = stopDistance * stopDistance;
        attackSqr = attackRange * attackRange;

        aiPath.maxSpeed = chaseSpeed;
        aiPath.slowdownDistance = Mathf.Max(slowdownDistance, stopDistance + 0.1f);
        aiPath.canSearch = false;
        aiPath.canMove = false;

        if (destSetter) destSetter.target = null;

        CachePlayerState();
    }

    void OnValidate()
    {
        if (exitChaseRadius < enterChaseRadius)
            exitChaseRadius = enterChaseRadius + 0.5f;
        if (hitRadius < 0f) hitRadius = 0f;
    }

    void CachePlayerState()
    {
        playerState = player ? player.GetComponent<PlayerState>() : null;
        if (!playerState)
            Debug.LogWarning("[Enemy] PlayerState not found on 'player' Transform!", player);
    }

    void Update()
    {
        if (!player) { EnterIdle(); return; }
        if (!playerState) CachePlayerState();

        float sqrDist = (player.position - transform.position).sqrMagnitude;

        switch (state)
        {
            case State.Idle:
                if (sqrDist <= enterSqr) EnterChase();
                break;

            case State.Chase:
                if (sqrDist > exitSqr) { EnterIdle(); break; }
                if (sqrDist <= attackSqr) { EnterAttack(); break; }

                aiPath.canMove = sqrDist > stopSqr; 
                break;

            case State.Attack:
                if (sqrDist > attackSqr && sqrDist <= exitSqr) { EnterChase(); break; }

                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    DoAttack();                
                    lastAttackTime = Time.time;
                }
                break;
        }
    }

    void EnterIdle()
    {
        state = State.Idle;
        aiPath.canSearch = false;
        aiPath.canMove = false;
        if (destSetter) destSetter.target = null;
        if (Enemyanim) Enemyanim.SetBool("isWalking", false);
    }

    void EnterChase()
    {
        state = State.Chase;
        aiPath.canSearch = true;
        if (destSetter) destSetter.target = player;
        if (Enemyanim) Enemyanim.SetBool("isWalking", true);
    }

    void EnterAttack()
    {
        state = State.Attack;
        aiPath.canMove = false; 
        if (Enemyanim)
        {
            Enemyanim.SetBool("isWalking", false);
            Enemyanim.SetTrigger("Attack");
        }
    }

    void DoAttack()
    {
        if (!playerState) { CachePlayerState(); if (!playerState) return; }


        Vector2 center = hitPoint ? (Vector2)hitPoint.position : (Vector2)transform.position;
        Collider2D hit = Physics2D.OverlapCircle(center, hitRadius, playerLayer);

        if (hit)
        {
        
            var ps = hit.GetComponent<PlayerState>();
            if (ps != null)
            {
                ps.TakeDamage(Damage);
             
            }
        }

        if (Enemyanim) Enemyanim.SetTrigger("Attack"); 
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 c = hitPoint ? hitPoint.position : transform.position;
        Gizmos.DrawWireSphere(c, hitRadius);
    }
}
