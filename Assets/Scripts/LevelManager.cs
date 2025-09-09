using System.Threading.Tasks;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject currentCheckPoint;
    public GameObject playerPrefab;
    public Transform spawnPoint;
   // [SerializeField] Unity.Cinemachine.CinemachineVirtualCamera virtualCamera;

    public void RespawnPlayer()
    {
        // Destroy old player if needed (optional, depends on your game logic)
        PlayerMovement oldPlayer = FindFirstObjectByType<PlayerMovement>();
        if (oldPlayer != null)
        {
            Destroy(oldPlayer.gameObject);
        }

        // Instantiate new player at checkpoint
        GameObject player = Instantiate(playerPrefab, currentCheckPoint.transform.position, currentCheckPoint.transform.rotation);

        // Re-set camera follow
      //  if (virtualCamera != null)
      //      virtualCamera.Follow = player.transform;

        // Update HealthBar reference
      //  PlayerStats playerStats = player.GetComponent<PlayerStats>();
      //  HealthBar healthBar = FindObjectOfType<HealthBar>();
      //  if (healthBar != null && playerStats != null)
      //      healthBar.SetPlayerHealth();
    }
    void Start()
    {
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        PlayerMovement oldPlayer = FindFirstObjectByType<PlayerMovement>();
       // playerStats playerStats = player.GetComponent<PlayerState>();
        HealthBar healthBar = FindFirstObjectByType<HealthBar>();
        //if (healthBar != null && playerStats != null)
        //{
        //    healthBar.SetPlayerHealth();
        //}
        //else
        //{
        //    Debug.Log("HealthBar or PlayerStats not found!");
        //}
      //  if (virtualCamera != null)
      //  {
      //      virtualCamera.Follow = player.transform;
      //  }
    }

}
