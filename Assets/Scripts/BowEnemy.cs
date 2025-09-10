using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BowEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float enemySpeed = 5f;
    [SerializeField] float stopDis = 6f;    // يقترب حتى هذه المسافة
    [SerializeField] float retreatDis = 3.5f;  // يهرب لو أقرب من هذه المسافة

    [Header("Target")]
    [SerializeField] Transform player;

    [Header("Shooting")]
    [SerializeField] GameObject arrowPrefab;   // Prefab السهم
    [SerializeField] Transform shootOrigin;    // نقطة خروج السهم (لو فاضي = this.transform)
    [SerializeField] float fireCooldown = 1.0f;
    [SerializeField] float arrowSpeed = 12f;
    [SerializeField] int arrowDamage = 10;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    float nextFireTime;

    // Animator params (اختياري)
    readonly int hashIsWalking = Animator.StringToHash("isWalking");
    readonly int hashAttack = Animator.StringToHash("Attack");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (!anim) anim = GetComponentInChildren<Animator>(true);
        sr = GetComponentInChildren<SpriteRenderer>();

        if (!shootOrigin) shootOrigin = transform;
        if (stopDis <= retreatDis) stopDis = retreatDis + 0.5f;

        if (!anim || anim.runtimeAnimatorController == null)
            Debug.LogWarning("[BowEnemy] Missing Animator/Controller.", this);
    }

    void Start()
    {
        if (!player)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) player = go.transform;
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
            // اقترب
            newPos = Vector2.MoveTowards(rb.position, player.position, enemySpeed * Time.fixedDeltaTime);
            isMoving = true;
        }
        else if (distance < retreatDis)
        {
            // اهرب
            Vector2 away = (rb.position - (Vector2)player.position).normalized;
            newPos = rb.position + away * enemySpeed * Time.fixedDeltaTime;
            isMoving = true;
        }
        else
        {
            // واقف في المدى المريح → اضرب باستمرار بكولداون
            TryShootWhileStanding();
            isMoving = false;
        }

        rb.MovePosition(newPos);

        // لف يمين/شمال ناحية اللاعب (اختياري)
        if (sr)
        {
            float dx = player.position.x - transform.position.x;
            if (Mathf.Abs(dx) > 0.01f) sr.flipX = dx < 0;
        }

        // أنيميشن مشي (لو المتغير موجود)
        if (anim) SafeSetBool(hashIsWalking, isMoving);
    }

    void TryShootWhileStanding()
    {
        if (!anim || !arrowPrefab || !player) return;

        // ماترميش Trigger جديد لو لسه كليب الضرب شغال (منع التداخل)
        if (IsInAttackAnim()) return;

        if (Time.time >= nextFireTime)
        {
            SafeSetTrigger(hashAttack);            // يبدأ كليب bowHolderShot
            nextFireTime = Time.time + fireCooldown;
        }
    }

    bool IsInAttackAnim()
    {
        if (!anim) return false;
        var st = anim.GetCurrentAnimatorStateInfo(0);
        // عدّلي الاسم لو كليبك اسمه مختلف
        return st.IsName("bowHolderShot") || st.tagHash == Animator.StringToHash("Shot");
    }

    // ===== تُنادى من Animation Event داخل كليب الضرب =====
    public void Anim_Shoot()
    {
        if (!arrowPrefab || !player) return;

        Vector2 dir = ((Vector2)player.position - (Vector2)shootOrigin.position).normalized;

        GameObject arrow = Instantiate(arrowPrefab, shootOrigin.position, Quaternion.identity);

        // وجّه السهم
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);

        // ادّي سرعة
        var rbArrow = arrow.GetComponent<Rigidbody2D>();
        if (rbArrow) rbArrow.linearVelocity = dir * arrowSpeed;

        // ادّي Damage لو السكربت موجود
        var proj = arrow.GetComponent<Projectile>();
        if (proj) proj.SetDamage(arrowDamage);
    }

    // Helpers آمنة (ما تشتغلش لو البراميتر مش موجود)
    void SafeSetBool(int hash, bool v)
    {
        if (!anim) return;
        foreach (var p in anim.parameters)
            if (p.type == AnimatorControllerParameterType.Bool && p.nameHash == hash)
            { anim.SetBool(hash, v); return; }
    }
    void SafeSetTrigger(int hash)
    {
        if (!anim) return;
        foreach (var p in anim.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger && p.nameHash == hash)
            { anim.SetTrigger(hash); return; }
        Debug.LogWarning("[BowEnemy] Missing Trigger 'Attack' on Animator.", this);
    }
}
