using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] PlayerState playerState;
    [SerializeField] Image HealthBarEmpty;
    [SerializeField] Image healthBarCurrent;
    private void Start()
    {
        HealthBarEmpty.fillAmount = 50f/100f;

    }

    private void Update()
    {
        healthBarCurrent.fillAmount = playerState.currentHealth / 100f;
    }
}