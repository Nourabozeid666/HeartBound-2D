using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    public bool ShieldIsActive = false;
    [SerializeField] Image barImage;
    [SerializeField] Sprite idleBarImage;
    [SerializeField] List<Sprite> shieldState = new List<Sprite>();
    [SerializeField] private float stepDelay = 1f;
    [SerializeField] string playerTag = "Player";
    int shownIndex = 0;
    public int targetIndexShield = 0;

    private PlayerState playerState;
    private float nextSearchTime = 0f;
    private const float searchInterval = 0.5f;

    Coroutine animCo = null;

    private void Update()
    {
        if (shownIndex != targetIndexShield)
        {
            if (animCo != null) 
                StopCoroutine(animCo);
            animCo = StartCoroutine(AnimateBar(shownIndex, targetIndexShield));
        }
        if (playerState == null && Time.time >= nextSearchTime)
        {
            TryFindPlayer();
            nextSearchTime = Time.time + searchInterval;
        }
    }
    public void RestoreHealthBar()
    {
        targetIndexShield = 0;
        shownIndex = 0;
        barImage.sprite = idleBarImage;
    }
    private void TryFindPlayer()
    {
        var go = GameObject.FindGameObjectWithTag(playerTag);
        if (go != null)
        {
            playerState = go.GetComponent<PlayerState>();
            if (playerState != null) return;
        }
        playerState = FindFirstObjectByType<PlayerState>(FindObjectsInactive.Include);
    }
    IEnumerator AnimateBar(int from, int to)
    {
        for (int i = from; i < to; i++)
        {
            shownIndex = i;
            barImage.sprite = shieldState[shownIndex];
            yield return new WaitForSeconds(stepDelay);
            if (shownIndex == to)
                break;
        }
        animCo = null;
    }
}
