using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image HealthBarEmpty;
    [SerializeField] Image healthBarCurrent;
    [SerializeField] string playerTag = "Player";  // make sure your player clone uses this tag

    private PlayerState playerState;
    private float nextSearchTime = 0f;
    private const float searchInterval = 0.5f;
    private void Start()
    {
        // set a default look so it isn't empty
        if (HealthBarEmpty) HealthBarEmpty.fillAmount = 1f;
        if (healthBarCurrent) healthBarCurrent.fillAmount = 0f;
    }

    private void Update()
    {
        // If we don't have a player yet, keep trying to find one
        if (playerState == null && Time.time >= nextSearchTime)
        {
            TryFindPlayer();
            nextSearchTime = Time.time + searchInterval;
        }

        // If we have a player now, update the bar
        if (playerState != null && healthBarCurrent != null)
        {
            // Use maxHealth if your PlayerState has it; otherwise assume 100
            int max = playerState.maxHealth > 0 ? playerState.maxHealth : 100;
            float pct = Mathf.Clamp01((float)playerState.currentHealth / max);
            healthBarCurrent.fillAmount = pct;
        }
    }
    private void TryFindPlayer()
    {
        // 1) Fast path: find an active Player by tag
        var go = GameObject.FindGameObjectWithTag(playerTag);
        if (go != null)
        {
            playerState = go.GetComponent<PlayerState>();
            if (playerState != null) return;
        }

        // 2) Fallback: find ANY PlayerState in the scene (including inactive)
        //    Unity 6: use FindFirstObjectByType with Include inactive
        playerState = FindFirstObjectByType<PlayerState>(FindObjectsInactive.Include);
        // If still null, we'll try again on the next tick (no errors, no spam).
    }
}