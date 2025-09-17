using UnityEngine;

public class gate : MonoBehaviour
{
    //[SerializeField] AudioClip travelPort;

    Collider2D col;

    bool locked = false;

    [SerializeField] bool bypassLevelLock = false;
    public bool BypassesLock => bypassLevelLock;
    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }
    public void SetLocked(bool val)
    {
        if (bypassLevelLock)
        {
            locked = false;
            col.enabled = true;
            return;
        }
        locked = val;
        col.enabled = !locked;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (bypassLevelLock)
            {
                SceneController.instance.NextLevel();
                return;
            }
            if(SceneController.instance && SceneController.instance.canExitLevel)
            {
                // AudioManager.instance.EnemyAction(travelPort);
                SceneController.instance.GoToNextLevel();

            }
        }
    }
}
