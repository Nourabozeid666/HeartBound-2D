using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class gate : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] gateAnim gateAnimator;   
    [SerializeField] bool isHubGate = false;


     Collider2D trig;

    void Awake()
    {
        trig = GetComponent<Collider2D>();
        

        if (!isHubGate)
        {
            trig.isTrigger = false;
        }
        else
        {
            trig.isTrigger = true;
        }
    }

  
    public void HandleGateOpened()
    {
        trig.isTrigger = true;

    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (!other.CompareTag("Player")) return;

        if (isHubGate)
        {
            SceneController.instance?.NextLevel();
            return;
        }

        if (SceneController.instance && SceneController.instance.CanExitLevel)
        {
            SceneController.instance.GoToNextLevel();
        }
    }

}
