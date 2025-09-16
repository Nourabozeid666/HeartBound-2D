using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Sprite HealthBarEmpty;
    [SerializeField] Image barImage;
    [SerializeField] string playerTag = "Player";  // make sure your player clone uses this tag
    [SerializeField] List <Sprite> healthState = new List<Sprite>();
    [SerializeField] private float stepDelay = 1f;
    int shownIndex = 0;
    bool isChanging = false;
    public int targetIndex = 0;
    Coroutine animCo = null;

    private PlayerState playerState;
    private float nextSearchTime = 0f;
    private const float searchInterval = 0.5f;
    private void Start()
    {
        HealthBarEmpty = healthState[29];
    }

    private void Update()
    {
        if (playerState == null && Time.time >= nextSearchTime)
        {
            TryFindPlayer();
            nextSearchTime = Time.time + searchInterval;
        }

        // If we have a player now, update the bar
        if (playerState != null && barImage != null &&  healthState != null && healthState.Count > 0 )
        {
            if(shownIndex != targetIndex)
            {
                //if (animCo != null) 
                //    StopCoroutine(animCo);
                animCo = StartCoroutine(AnimateBar(shownIndex, targetIndex));
            }
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
        playerState = FindFirstObjectByType<PlayerState>(FindObjectsInactive.Include);
    }

    IEnumerator AnimateBar(int from, int to)
    {
        for (int i = from; i < to; i++)
        {
            shownIndex = i;
            barImage.sprite = healthState[shownIndex];
            isChanging = true;
            yield return new WaitForSeconds(stepDelay);
            isChanging = false;
            if (shownIndex == to)
                break;
        }
       // animCo = null;
    }
}