using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BowEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float enemySpeed = 5f;
    [SerializeField] float stopDis = 6f;      
    [SerializeField] float retreatDis = 3.5f;  

    [Header("Target")]
    [SerializeField] Transform player;

    [Header("Shooting")]
    [SerializeField] GameObject arrowPrefab;    
    [SerializeField] Transform shootOrigin;     
    [SerializeField] float fireCooldown = 1.0f; 
    [SerializeField] float arrowSpeed = 12f;
    [SerializeField] int arrowDamage = 10;


    Rigidbody2D rb;
    float nextFireTime;
    bool inComfortRange;
    Animator anim;

    bool driveWalkAnim = true;
    bool driveShootAnim = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>(true);

        if (stopDis <= retreatDis) stopDis = retreatDis + 0.5f;
    }

    void Start()
    {
        if (!player)
        {
            var GetPlayer = GameObject.FindGameObjectWithTag("Player");
            if (GetPlayer) player = GetPlayer.transform;
        }
    }

    void FixedUpdate()
    {
        if (!player) return;

        float distance = Vector2.Distance(rb.position, player.position);
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
            anim.SetBool("isWalking", isMoving);

        if (anim && driveShootAnim && anim.HasParameterOfType("isShooting", AnimatorControllerParameterType.Bool))
            anim.SetBool("isShooting", inComfortRange);
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

        Vector2 origin = shootOrigin ? (Vector2)shootOrigin.position : (Vector2)transform.position;

        Vector2 dir = ((Vector2)player.position - origin).normalized;

        GameObject arrow = Instantiate(arrowPrefab, origin, Quaternion.identity);
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);

        var rbArrow = arrow.GetComponent<Rigidbody2D>();
        if (rbArrow) rbArrow.linearVelocity = dir * arrowSpeed;  

        var proj = arrow.GetComponent<Projectile>();
        if (proj) proj.SetDamage(arrowDamage);

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
