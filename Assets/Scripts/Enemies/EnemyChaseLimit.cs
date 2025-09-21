using Pathfinding;
using UnityEngine;

public class EnemyChaseLimit : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] AIPath aiPath;
    [SerializeField] AIDestinationSetter destSetter;
    [SerializeField] Animator anim; 

    [Header("Chase")]
    [SerializeField] float enterChaseRadius = 8f;
    [SerializeField] float exitChaseRadius = 10f;

    [Header("Attack Distances")]
    [SerializeField] float stopDistance = 1.5f; 
    [SerializeField] float attackRange = 1.0f; 

    [Header("Attack Timing")]
    [SerializeField] float attackWindup = 0.20f;
    [SerializeField] float attackActive = 0.10f;
    [SerializeField] float attackRecovery = 0.40f;
    [SerializeField] float attackCooldown = 1.00f;

    [Header("Damage")]
    [SerializeField] int Damage = 10;

    [Header("Melee Hitbox")]
    [SerializeField] Transform hitPoint;
    [SerializeField] float hitRadius = 0.6f;
    [SerializeField] LayerMask playerLayer;

    [Header("Pathfinding")]
    [SerializeField] float slowdownDistance = 3f;
    [SerializeField] float chaseSpeed = 3.5f;

    enum State { Idle, Chase, Attack }
    State state = State.Idle;

    float enterSqr, exitSqr, stopSqr, attackSqr;

    bool isAttacking = false;
    bool didHitThisSwing = false;
    float attackStartTime = -999f;
    float nextAttackAllowedTime = 0f;

    Transform player;
    PlayerState playerState;


    readonly int hashIsWalking = Animator.StringToHash("isWalking");
    readonly int hashAttack = Animator.StringToHash("Attack");




    void Awake()
    {
        if (!aiPath) aiPath = GetComponent<AIPath>();
        if (!destSetter) destSetter = GetComponent<AIDestinationSetter>();
        if (!anim) anim = GetComponentInChildren<Animator>(true);

        enterSqr = enterChaseRadius * enterChaseRadius;
        exitSqr = exitChaseRadius * exitChaseRadius;
        stopSqr = stopDistance * stopDistance;
        attackSqr = attackRange * attackRange;

        if (aiPath)
        {
            aiPath.maxSpeed = chaseSpeed;
            aiPath.slowdownDistance = Mathf.Max(slowdownDistance, stopDistance + 0.1f);
            aiPath.canSearch = false;
            aiPath.canMove = false;
        }
        if (destSetter) destSetter.target = null;
    }

    void Start()
    {
        var pm = FindFirstObjectByType<PlayerMovement>();
        if (pm) player = pm.transform;
        CachePlayerState();
    }

    void OnValidate()
    {
        if (exitChaseRadius < enterChaseRadius) exitChaseRadius = enterChaseRadius + 0.5f;
        if (hitRadius < 0f) hitRadius = 0f;
        if (attackWindup < 0f) attackWindup = 0f;
        if (attackActive < 0f) attackActive = 0f;
        if (attackRecovery < 0f) attackRecovery = 0f;
        if (attackCooldown < 0f) attackCooldown = 0f;
    }

    void CachePlayerState()
    {
        playerState = player ? player.GetComponent<PlayerState>() : null;
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
                if (sqrDist <= attackSqr)
                {
                    EnterAttack();
                    break;
                }

          
                if (aiPath) aiPath.canMove = sqrDist > stopSqr;
                break;

            case State.Attack:
   
                if (sqrDist > attackSqr && sqrDist <= exitSqr) { EnterChase(); break; }

                UpdateAttackTiming();
                break;
        }
    }

    void EnterIdle()
    {
        state = State.Idle;
        isAttacking = false;
        didHitThisSwing = false;

        if (aiPath)
        {
            aiPath.canSearch = false;
            aiPath.canMove = false;
        }
        if (destSetter) destSetter.target = null;
        if (anim) anim.SetBool(hashIsWalking, false);
    }

    void EnterChase()
    {
        state = State.Chase;
        isAttacking = false;
        didHitThisSwing = false;

        if (aiPath) aiPath.canSearch = true;
        if (destSetter) destSetter.target = player;
        if (anim) anim.SetBool(hashIsWalking, true);
    }

    void EnterAttack()
    {
        state = State.Attack;

        if (Time.time < nextAttackAllowedTime)
        {
            if (aiPath) aiPath.canMove = false;
            if (anim) anim.SetBool(hashIsWalking, false);
            return;
        }

        isAttacking = true;
        didHitThisSwing = false;
        attackStartTime = Time.time;
        nextAttackAllowedTime = Time.time + attackCooldown; 

        if (aiPath) aiPath.canMove = false; 
        if (anim)
        {
            anim.SetBool(hashIsWalking, false);
            anim.ResetTrigger(hashAttack);
            anim.SetTrigger(hashAttack);   
        }
    }

    void UpdateAttackTiming()
    {
        if (!isAttacking)
        {
            return;
        }

   
        float tNow = Time.time - attackStartTime;
        float tPrev = tNow - Time.deltaTime;

  
        float start = attackWindup;
        float end = attackWindup + attackActive;


        bool crossedWindow = (tPrev < end) && (tNow >= start);

        if (!didHitThisSwing && crossedWindow)
        {
            DoAttackHit();     
            didHitThisSwing = true;
        }

       
        if (tNow >= attackWindup + attackActive + attackRecovery)
        {
            isAttacking = false;
        }
    }


    void DoAttackHit()
    {
   
        Vector2 center = hitPoint ? (Vector2)hitPoint.position : (Vector2)transform.position;

        var hits = Physics2D.OverlapCircleAll(center, hitRadius, playerLayer);

        foreach (var h in hits)
        {
 
            var ps = h.GetComponentInParent<PlayerState>();
            if (ps == null) continue;

            ps.TakeDamage(Damage);
            break; 
        }
    }




    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 c = hitPoint ? hitPoint.position : transform.position;
        Gizmos.DrawWireSphere(c, hitRadius);
    }
}
