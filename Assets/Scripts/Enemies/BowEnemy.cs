using System.Collections;
using UnityEngine;

public class BowEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float enemySpeed = 5f;
    [SerializeField] float stopDis = 6f;
    [SerializeField] float retreatDis = 3.5f;

    [Header("Engage Range")]
    [SerializeField] float engageRadius = 10f;
    [SerializeField] float disengageRadius = 12f;

    [Header("Shooting")]
    [SerializeField] GameObject arrowPrefab;        
    [SerializeField] Transform shootOrigin;
    [SerializeField] float fireCooldown = 0.6f;     
    [SerializeField] float arrowSpeed = 12f;
    [SerializeField] int arrowDamage = 10;
    [SerializeField] float spawnOffset = 0.6f;      

    [Header("Animator (optional)")]
    [SerializeField] Animator anim;
    [SerializeField] bool driveWalkAnim = true;
    [SerializeField] bool driveShootAnim = true;

    [Header("Debug")]
    [SerializeField] bool debugShoot = false;

    readonly int hashIsWalking = Animator.StringToHash("isWalking");
    readonly int hashIsShooting = Animator.StringToHash("isShooting");

    Transform player;
    Rigidbody2D rb;
    float nextFireTime;
    bool engaged;

    EnemyHealth eh;     
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!anim) anim = GetComponentInChildren<Animator>(true);
        eh = GetComponentInParent<EnemyHealth>() ?? GetComponent<EnemyHealth>(); 


        if (stopDis <= retreatDis) stopDis = retreatDis + 0.5f;
        if (disengageRadius < engageRadius) disengageRadius = engageRadius + 1f;
        if (engageRadius < stopDis) engageRadius = stopDis + 0.5f;

        if (!shootOrigin) shootOrigin = transform;
    }

    void OnEnable()
    {
        TryAssignPlayerOnce();
        if (!player) StartCoroutine(RetryFindPlayer());

        if (eh != null) eh.OnDead += HandleDead;
    }

    void OnDisable()    
    {
        if (eh != null) eh.OnDead -= HandleDead;
    }

    void HandleDead(EnemyHealth _)   
    {
  
        nextFireTime = float.PositiveInfinity;   
        if (anim)
        {
   
            if (anim.parameters != null)
            {
                if (anim.HasParameterOfType("isShooting", AnimatorControllerParameterType.Bool))
                    anim.SetBool(hashIsShooting, false);
                if (anim.HasParameterOfType("isWalking", AnimatorControllerParameterType.Bool))
                    anim.SetBool(hashIsWalking, false);
            }
        }
        enabled = false;  
    }
    void Start()
    {
        if (!arrowPrefab)
            Debug.LogWarning("[BowEnemy] arrowPrefab not assigned on the PREFAB!", this);
    }

    IEnumerator RetryFindPlayer()
    {
 
        for (int i = 0; i < 10 && !player; i++)
        {
            yield return null;
            TryAssignPlayerOnce();
        }
        if (!player)
        {
            yield return new WaitForSeconds(0.2f);
            TryAssignPlayerOnce();
        }
    }

    void TryAssignPlayerOnce()
    {
        var go = GameObject.FindWithTag("Player");
        if (go) { player = go.transform; return; }

        var pm = FindFirstObjectByType<PlayerMovement>();
        if (pm) player = pm.transform;
    }

    void FixedUpdate()
    {
  
        if (!player) return;

        if (eh && (eh.IsDead || eh.IsStunned))
        {
            if (anim && driveWalkAnim && anim.HasParameterOfType("isWalking", AnimatorControllerParameterType.Bool))
                anim.SetBool(hashIsWalking, false);
            if (anim && driveShootAnim && anim.HasParameterOfType("isShooting", AnimatorControllerParameterType.Bool))
                anim.SetBool(hashIsShooting, false);
            return;
        }


        float distance = Vector2.Distance(rb.position, player.position);


        if (!engaged && distance <= engageRadius) engaged = true;
        else if (engaged && distance > disengageRadius) engaged = false;

        if (!engaged)
        {
            SetAnim(false, false);
            UpdateFacing();
            return;
        }

        Vector2 newPos = rb.position;
        bool isMoving = false;

        if (distance > stopDis)
        {
            newPos = Vector2.MoveTowards(rb.position, player.position, enemySpeed * Time.fixedDeltaTime);
            isMoving = true;
        }
        else if (distance < retreatDis)
        {
            Vector2 away = (rb.position - (Vector2)player.position).normalized;
            newPos = rb.position + away * enemySpeed * Time.fixedDeltaTime;
            isMoving = true;
        }

        rb.MovePosition(newPos);
        UpdateFacing();

      
        TryShootOnTimer();


        SetAnim(isMoving, !isMoving);
    }

    void SetAnim(bool isWalking, bool isShooting)
    {
        if (anim && driveWalkAnim && anim.HasParameterOfType("isWalking", AnimatorControllerParameterType.Bool))
            anim.SetBool(hashIsWalking, isWalking);

        if (anim && driveShootAnim && anim.HasParameterOfType("isShooting", AnimatorControllerParameterType.Bool))
            anim.SetBool(hashIsShooting, isShooting);
    }

    void UpdateFacing()
    {
        if (!player) return;
        int face = (player.position.x >= transform.position.x) ? 1 : -1;
        var s = transform.localScale;
        s.x = Mathf.Abs(s.x) * face;
        transform.localScale = s;
    }

    void TryShootOnTimer()
    {
        if (eh && (eh.IsDead || eh.IsStunned)) return;
        if (!player) return;
        if (!arrowPrefab)
        {
            if (debugShoot) Debug.LogError("[BowEnemy] arrowPrefab is NULL! اربطيه على الPrefab.", this);
            return;
        }
        if (Time.time < nextFireTime) return;

        Vector2 origin = (Vector2)(shootOrigin ? shootOrigin.position : transform.position);
        Vector2 toTarget = (Vector2)player.position - origin;
        if (toTarget.sqrMagnitude < 0.0001f) return;

        Vector2 dir = toTarget.normalized;
        Vector2 spawnPos = origin + dir * spawnOffset;

        GameObject arrow = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);

        var arrowCol = arrow.GetComponent<Collider2D>();
        if (arrowCol)
        {
            var myCols = GetComponentsInChildren<Collider2D>();
            foreach (var c in myCols) Physics2D.IgnoreCollision(arrowCol, c, true);
        }

        var proj = arrow.GetComponent<Projectile>();
        if (proj) proj.SetDamage(arrowDamage);

        var rb2d = arrow.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.gravityScale = 0f;
            rb2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb2d.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb2d.linearVelocity = dir * arrowSpeed; 
        }
        else
        {
            if (!proj) proj = arrow.AddComponent<Projectile>();
            proj.Init(dir, arrowSpeed);
        }

        nextFireTime = Time.time + fireCooldown;
    }

}

static class AnimatorParamCheck
{
    public static bool HasParameterOfType(this Animator a, string name, AnimatorControllerParameterType type)
    {
        if (!a) return false;
        foreach (var p in a.parameters)
            if (p.type == type && p.name == name) return true;
        return false;
    }
}
