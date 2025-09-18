using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image barImage;
    [SerializeField] Sprite idleBarImage;
    [SerializeField] string playerTag = "Player"; 
    [SerializeField] List <Sprite> healthState = new List<Sprite>();
    [SerializeField] private float stepDelay = 1f;
    int shownIndex = 0;
    public int targetIndex = 0;
    Coroutine animCo = null;

    private PlayerState playerState;
    private float nextSearchTime = 0f;
    private const float searchInterval = 0.5f;

    private void Update()
    {
        if (playerState == null && Time.time >= nextSearchTime)
        {
            TryFindPlayer();
            nextSearchTime = Time.time + searchInterval;
        }

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

    public void RestoreHealthBar()
    {
        targetIndex = 0;
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
            barImage.sprite = healthState[shownIndex];
            yield return new WaitForSeconds(stepDelay);
            if (shownIndex == to)
                break;
        }
       // animCo = null;
    }

}