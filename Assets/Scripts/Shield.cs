using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    public bool ShieldIsActive = false;
    //Sprite HealthBarEmpty;
    [SerializeField] Image barImage;
    [SerializeField] Sprite idleBarImage;
    [SerializeField] List<Sprite> healthState = new List<Sprite>();
    [SerializeField] private float stepDelay = 1f;

    int shownIndex = 0;
    public int targetIndex = 0;

    Coroutine animCo = null;

    private void Update()
    {
        if (shownIndex != targetIndex)
        {
            animCo = StartCoroutine(AnimateBar(shownIndex, targetIndex));
        }
    }
    public void RestoreHealthBar()
    {
        targetIndex = 0;
        shownIndex = 0;
        barImage.sprite = idleBarImage;
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
        animCo = null;
    }
}
