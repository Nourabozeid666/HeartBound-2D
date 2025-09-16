using UnityEngine;

public class gate : MonoBehaviour
{
    //[SerializeField] AudioClip travelPort;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           // AudioManager.instance.EnemyAction(travelPort);
            SceneController.instance.NextLevel();
        }
    }
}
