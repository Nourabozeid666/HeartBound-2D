using System.Collections.Generic;
using UnityEngine;

public class SpowningEnemies : MonoBehaviour
{
    [Header("Spawn setup")]
    public Transform[] enemiesPositions;   // assign spawn points in Inspector
    public GameObject[] enemies;           // assign enemy PREFABS in Inspector

    [Header("Optional")]
    public Transform parentForSpawns;      // keep hierarchy tidy (optional)

    private readonly List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        ResetAllEnemiess();
    }

    public void ResetAllEnemiess()
    {
        // Validate inputs
        if (enemies == null || enemies.Length == 0)
        {
            Debug.LogError("[Spawner] 'enemies' array is empty. Assign enemy PREFABS in the Inspector.");
            return;
        }
        if (enemiesPositions == null || enemiesPositions.Length == 0)
        {
            Debug.LogError("[Spawner] 'enemiesPositions' array is empty. Assign spawn points in the Inspector.");
            return;
        }

        // Spawn at most the available pairs of (enemy, position)
        int spawnCount = Mathf.Min(enemies.Length, enemiesPositions.Length);

        // (Optional) clean up previously spawned enemies
        for (int i = 0; i < spawnedEnemies.Count; i++)
            if (spawnedEnemies[i]) Destroy(spawnedEnemies[i]);
        spawnedEnemies.Clear();

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject prefab = enemies[i];
            Transform at = enemiesPositions[i];

            if (prefab == null || at == null)
            {
                Debug.LogWarning($"[Spawner] Skipping index {i}: prefab or spawn point is null.");
                continue;
            }

            // Instantiate prefab at position/rotation; parent is optional
            GameObject instance = Instantiate(prefab, at.position, at.rotation, parentForSpawns);

            // If your prefab was saved inactive, ensure the instance is active
            if (!instance.activeSelf) instance.SetActive(true);

            spawnedEnemies.Add(instance);
            // Debug.Log($"Spawned: {instance.name} at {at.position}");
        }
    }
}
