
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
  
 
    [Header("Enemy Prefabs")]
    [SerializeField] List<GameObject> enemyPrefabs;

    [Header("Spawn Areas")]
    [SerializeField] BoxCollider2D[] boxAreas;   

    [Header("Placement Rules")]
    [SerializeField] float minDistanceFromPlayer = 2.0f;
    [SerializeField] float clearanceRadius = 0.4f;
    [SerializeField] LayerMask blockedMask;
    [SerializeField] int maxAttempts = 25;

    [Header("Options")]
    [SerializeField] bool randomizeEnemyType = true;
    [SerializeField] float spawnInterval = 0.1f;


    Transform player;

    // To use in Level Spawner
    public int AliveInBatch { get; private set; }
    public int SpawnedInBatch { get; private set; }
    public int LastBatchSize { get; private set; }
    public bool IsSpawning { get; private set; }

    public event Action<int> OnBatchCleared;
    public event Action OnEnemySpawned;
    public event Action OnEnemyDied;


    [Header("Auto Test")]
    [SerializeField] bool autoTestOnPlay = false; 
    [SerializeField] int autoTestCount = 3;    
    [SerializeField] int autoTestRepeats = 1;    
    [SerializeField] float autoTestRepeatDelay = 1f; 

    

    IEnumerator Start()
    {
        if (autoTestOnPlay)
            yield return StartCoroutine(AutoTestRoutine());
    }

    IEnumerator AutoTestRoutine()
    {
        for (int i = 0; i < autoTestRepeats; i++)
        {
            BeginBatch(autoTestCount); 
            yield return new WaitUntil(() => AliveInBatch == 0 && !IsSpawning);
            if (i < autoTestRepeats - 1)
                yield return new WaitForSeconds(autoTestRepeatDelay);
        }
    }


    void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>().transform;

        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
            Debug.LogWarning("[EnemyBatchSpawner] No enemyPrefabs assigned.", this);

        if (boxAreas == null || boxAreas.Length == 0)
            Debug.LogWarning("[EnemyBatchSpawner] No spawn areas assigned.", this);

        if (maxAttempts < 1) maxAttempts = 1;
    }

    public Coroutine BeginBatch(int count)
    {
        if (count <= 0)
        {
            Debug.LogWarning("[EnemyBatchSpawner] BeginBatch called with count <= 0", this);
            return null;
        }
        return StartCoroutine(SpawnBatchRoutine(count));
    }

    IEnumerator SpawnBatchRoutine(int count)
    {
        IsSpawning = true;
        LastBatchSize = count;
        SpawnedInBatch = 0;
        AliveInBatch = 0;

        for (int i = 0; i < count; i++)
        {
            bool ok = SpawnOne();
            if (!ok)
                Debug.LogWarning($"[EnemyBatchSpawner] Failed to spawn enemy #{i + 1}/{count}", this);

            if (spawnInterval > 0f) yield return new WaitForSeconds(spawnInterval);
        }

        IsSpawning = false;

        while (AliveInBatch > 0)
            yield return null;

        OnBatchCleared?.Invoke(LastBatchSize);
    }

    bool SpawnOne()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0) return false;
        if (boxAreas == null || boxAreas.Length == 0) return false;

        var prefab = randomizeEnemyType
            ? enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)]
            : enemyPrefabs[0];

        Vector2 pos;
        if (!TryGetValidPoint(out pos))
        {
            Debug.LogWarning("[EnemyBatchSpawner] Could not find a valid spawn point after attempts.", this);
            return false;
        }

        var enemy = Instantiate(prefab, pos, Quaternion.identity);

        var hp = enemy.GetComponentInChildren<EnemyHealth>(true);

        AliveInBatch++;
        SpawnedInBatch++;
        OnEnemySpawned?.Invoke();

        if (hp != null)
        {
            hp.OnDead += HandleEnemyDead;
        }
        else
        {
            var notifier = enemy.AddComponent<_DestroyNotifier>();
            notifier.onDestroyed += HandleEnemyDestroyedNoHealth;
        }
        return true;
    }


    bool TryGetValidPoint(out Vector2 result)
    {
       
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            bool got = false;
            Vector2 p = Vector2.zero;

            bool tryBoxFirst = (boxAreas != null && boxAreas.Length > 0) 
                               ? (UnityEngine.Random.value < 0.5f)
                               : (boxAreas != null && boxAreas.Length > 0);

            if (tryBoxFirst)
                got = TryPointInRandomBox(out p);
            if (!got && boxAreas != null && boxAreas.Length > 0)
                got = TryPointInRandomBox(out p);

            if (!got) continue;

          
            if (player && Vector2.Distance(p, player.position) < minDistanceFromPlayer)
                continue;

           
            if (clearanceRadius > 0f && blockedMask.value != 0)
            {
                if (Physics2D.OverlapCircle(p, clearanceRadius, blockedMask))
                    continue;
            }

            result = p;
            return true;
        }

        result = Vector2.zero;
        return false;
    }

    bool TryPointInRandomBox(out Vector2 p)
    {
        p = Vector2.zero;
        if (boxAreas == null || boxAreas.Length == 0) return false;

        var box = boxAreas[UnityEngine.Random.Range(0, boxAreas.Length)];
        if (!box) return false;

       
        Vector2 half = box.size * 0.5f;
        Vector2 local = new Vector2(
            UnityEngine.Random.Range(-half.x, half.x),
            UnityEngine.Random.Range(-half.y, half.y)
        );

      
        Vector2 localWithOffset = box.offset + local;
        p = box.transform.TransformPoint(localWithOffset);
        return true;
    }


    void HandleEnemyDead(EnemyHealth eh)
    {
        if (eh != null) eh.OnDead -= HandleEnemyDead;
        AliveInBatch = Mathf.Max(AliveInBatch - 1, 0);
        OnEnemyDied?.Invoke();
    }

    void HandleEnemyDestroyedNoHealth(GameObject go)
    {
        var n = go.GetComponent<_DestroyNotifier>();
        if (n) n.onDestroyed -= HandleEnemyDestroyedNoHealth;
        AliveInBatch = Mathf.Max(AliveInBatch - 1, 0);
        OnEnemyDied?.Invoke();
    }


    class _DestroyNotifier : MonoBehaviour
    {
        public Action<GameObject> onDestroyed;
        void OnDestroy() { onDestroyed?.Invoke(gameObject); }
    }
}