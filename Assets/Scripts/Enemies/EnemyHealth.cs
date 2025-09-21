using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding; 

public class EnemyHealth : MonoBehaviour
{
    public event Action<EnemyHealth> OnDead;

    [Header("Health")]
    [SerializeField] float health = 30f;
    float currentHealth;
    public bool IsDead { get; private set; }
    public bool IsStunned { get; private set; }

    [Header("Animation")]
    [SerializeField] Animator animator;               
    [SerializeField] string deathStateName = "";     

    [Header("Hurt / Stun")]
    [SerializeField] bool useHurtAnimLength = true;
    [SerializeField] float fallbackStunDuration = 0.35f;


    AIPath aiPath;
    Seeker seeker;
    AIDestinationSetter destSetter;
    Rigidbody2D rb;
    RigidbodyConstraints2D originalConstraints;


    readonly List<MonoBehaviour> autoBehaviours = new();

    Coroutine stunCo;

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>(true);

     
        rb = GetComponent<Rigidbody2D>() ?? GetComponentInParent<Rigidbody2D>();
        aiPath = GetComponent<AIPath>() ?? GetComponentInParent<AIPath>();
        seeker = GetComponent<Seeker>() ?? GetComponentInParent<Seeker>();
        destSetter = GetComponent<AIDestinationSetter>() ?? GetComponentInParent<AIDestinationSetter>();

 
        CollectBehaviours(this.transform);
    }

    void CollectBehaviours(Transform t)
    {
        var parentScripts = t.GetComponentsInParent<MonoBehaviour>(true);
        var childScripts = t.GetComponentsInChildren<MonoBehaviour>(true);

        void AddRange(MonoBehaviour[] arr)
        {
            foreach (var mb in arr)
            {
                if (!mb) continue;
                if (mb == this) continue;
                if (animator && mb == animator) continue;
                if (mb is AIPath || mb is Seeker || mb is AIDestinationSetter) continue;
                autoBehaviours.Add(mb);
            }
        }

        AddRange(parentScripts);
        AddRange(childScripts);
    }

    void Start() => currentHealth = health;

    // ================== Damage ==================
    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
            return;
        }

        // Hurt + Stun
        IsStunned = true;
        if (animator) animator.SetBool("isHurt", true);

        if (stunCo != null) StopCoroutine(stunCo);
        stunCo = StartCoroutine(StunRoutine());
    }

    IEnumerator StunRoutine()
    {
        PauseMovement();

        float t = GetHurtDuration();
        yield return new WaitForSeconds(t);

        if (!IsDead && animator) animator.SetBool("isHurt", false);
        IsStunned = false;

        if (!IsDead) ResumeMovement();
        stunCo = null;
    }

    float GetHurtDuration()
    {
        if (useHurtAnimLength && animator)
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.length > 0.01f) return info.length;
        }
        return fallbackStunDuration;
    }

    // ================== Death ==================
    void Die()
    {
        IsDead = true;
        IsStunned = false;

        if (stunCo != null) StopCoroutine(stunCo);

        StopAllBehavioursHard();

        if (animator)
        {
    
            animator.ResetTrigger("Hurt");
            animator.SetBool("isHurt", false);

        
            animator.SetBool("isDead", true);

    
            if (!string.IsNullOrEmpty(deathStateName))
            {
                animator.Play(deathStateName, 0, 0f);
            }
        }

        OnDead?.Invoke(this);
        StartCoroutine(DeathCleanup(1f)); 
    }

    // ================== Movement Control ==================
    void PauseMovement()
    {
        if (aiPath)
        {
            aiPath.isStopped = true;
            aiPath.canMove = false;
            aiPath.canSearch = false;
        }
        if (seeker) seeker.CancelCurrentPathRequest();
        if (destSetter) destSetter.target = null;

        if (rb)
        {
            originalConstraints = rb.constraints;
            rb.linearVelocity = Vector2.zero;     
            rb.angularVelocity = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        foreach (var mb in autoBehaviours) if (mb) mb.enabled = false;
    }

    void ResumeMovement()
    {
        if (rb) rb.constraints = originalConstraints;

        if (aiPath)
        {
            aiPath.isStopped = false;
            aiPath.canMove = true;
            aiPath.canSearch = true;
        }

        foreach (var mb in autoBehaviours) if (mb) mb.enabled = true;

        if (destSetter && !destSetter.target)
        {
            var player = GameObject.FindWithTag("Player");
            if (player) destSetter.target = player.transform;
        }
    }

    void StopAllBehavioursHard()
    {
        foreach (var mb in autoBehaviours) if (mb) mb.enabled = false;

        if (aiPath)
        {
            aiPath.isStopped = true;
            aiPath.canMove = false;
            aiPath.canSearch = false;
            aiPath.enabled = false;
        }
        if (seeker) seeker.enabled = false;
        if (destSetter)
        {
            destSetter.target = null;
            destSetter.enabled = false;
        }

        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.simulated = false; 
        }
    }

    IEnumerator DeathCleanup(float delay)
    {
        yield return new WaitForSeconds(delay);
        var root = transform.root ? transform.root.gameObject : gameObject;
        Destroy(root);
    }
}
