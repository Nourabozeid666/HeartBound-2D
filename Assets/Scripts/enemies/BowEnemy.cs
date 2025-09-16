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
    [SerializeField] float fireCooldown = 1.0f;
    [SerializeField] float arrowSpeed = 12f;
    [SerializeField] int arrowDamage = 10;

    [Header("Animator (optional)")]

    [SerializeField] Animator anim;             
    [SerializeField] bool driveWalkAnim = true;  
    [SerializeField] bool driveShootAnim = true;  



    readonly int hashIsWalking = Animator.StringToHash("isWalking");
    readonly int hashIsShooting = Animator.StringToHash("isShooting");

    Transform player;
    Rigidbody2D rb;
    float nextFireTime;
    bool inComfortRange;
    bool engaged; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!anim) anim = GetComponentInChildren<Animator>(true);

    
        if (stopDis <= retreatDis) stopDis = retreatDis + 0.5f;
        if (disengageRadius < engageRadius) disengageRadius = engageRadius + 1f;
        if (engageRadius < stopDis) engageRadius = stopDis + 0.5f;

        if (!shootOrigin) shootOrigin = transform;
    }

    void OnValidate()
    {
        if (stopDis <= retreatDis) stopDis = retreatDis + 0.5f;
        if (disengageRadius < engageRadius) disengageRadius = engageRadius + 0.5f;
        if (engageRadius < stopDis) engageRadius = stopDis + 0.5f;
    }

    void Start()
    {
       player = FindFirstObjectByType<PlayerMovement>().transform;  
    }

    void FixedUpdate()
    {
        if (!player) return;

        float distance = Vector2.Distance(rb.position, player.position);


        if (!engaged && distance <= engageRadius) engaged = true;
        else if (engaged && distance > disengageRadius) engaged = false;


        if (!engaged)
        {
            inComfortRange = false;

            if (anim && driveWalkAnim && anim.HasParameterOfType("isWalking", AnimatorControllerParameterType.Bool))
                anim.SetBool(hashIsWalking, false);

            UpdateFacing();

            return;
        }

        Vector2 newPos = rb.position;
        bool isMoving = false;

        if (distance > stopDis)
        {

            newPos = Vector2.MoveTowards(rb.position, player.position, enemySpeed * Time.fixedDeltaTime);
            isMoving = true;
            inComfortRange = false;
        }
        else if (distance < retreatDis)
        {

            Vector2 away = (rb.position - (Vector2)player.position).normalized;
            newPos = rb.position + away * enemySpeed * Time.fixedDeltaTime;
            isMoving = true;
            inComfortRange = false;
        }
        else
        {

            inComfortRange = true;
            TryShootOnTimer();
            isMoving = false;
        }

        rb.MovePosition(newPos);
        UpdateFacing();


        if (anim && driveWalkAnim && anim.HasParameterOfType("isWalking", AnimatorControllerParameterType.Bool))
            anim.SetBool(hashIsWalking, isMoving);

        if (anim && driveShootAnim && anim.HasParameterOfType("isShooting", AnimatorControllerParameterType.Bool))
            anim.SetBool(hashIsShooting, inComfortRange);
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
        if (!arrowPrefab || !player) return;
        if (Time.time < nextFireTime) return;

        Vector2 origin = (Vector2)(shootOrigin ? shootOrigin.position : transform.position);
        Vector2 toTarget = (Vector2)player.position - origin;

        if (toTarget.sqrMagnitude < 0.0001f) return; 
        Vector2 dir = toTarget.normalized;
        GameObject arrow = Instantiate(arrowPrefab, origin, Quaternion.identity);

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
            rb2d.AddForce(dir * arrowSpeed, ForceMode2D.Impulse);
        }
        else
        {
            var mover = arrow.GetComponent<Projectile>();
            if (!mover) mover = arrow.AddComponent<Projectile>();
            mover.Init(dir, arrowSpeed);
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