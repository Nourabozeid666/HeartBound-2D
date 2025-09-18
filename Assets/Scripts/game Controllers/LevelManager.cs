using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineCamera cinemachineCam;
    [SerializeField] private GameObject hudCanvas;

    private GameObject player;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        var mainCam = Camera.main;
        if (mainCam) DontDestroyOnLoad(mainCam.gameObject);
        if (cinemachineCam) DontDestroyOnLoad(cinemachineCam.gameObject);
        if (hudCanvas) DontDestroyOnLoad(hudCanvas);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void Start()
    {
        if (player == null && playerPrefab != null)
        {
            var created = Instantiate(playerPrefab);
            RegisterPlayer(created);              // ← use Register here
        }
        else
        {
            SnapPlayerToSceneSpawn();
            RetargetCameraToPlayer();
        }

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If the scene already has a Player (e.g., spawned by your scene logic), adopt it.
        var scenePlayer = GameObject.FindGameObjectWithTag("Player");
        if (scenePlayer != null && scenePlayer != player)
        {
            RegisterPlayer(scenePlayer);
            return;
        }

        if (player == null && playerPrefab != null)
        {
            var created = Instantiate(playerPrefab);
            RegisterPlayer(created);
            return;
        }

        SnapPlayerToSceneSpawn();
        RetargetCameraToPlayer();
    }

    public void RegisterPlayer(GameObject newPlayer)
    {
        if (!newPlayer) return;

        if (player != null && player != newPlayer)
            Destroy(player);                      // keep only one persistent player

        player = newPlayer;
        DontDestroyOnLoad(player);

        SnapPlayerToSceneSpawn();
        RetargetCameraToPlayer();
    }

    private void SnapPlayerToSceneSpawn()
    {
        if (!player) return;
        var spawn = FindFirstObjectByType<SpownPoint>(); // ← spell exactly like your script/class
        if (spawn)
            player.transform.SetPositionAndRotation(spawn.transform.position, spawn.transform.rotation);
    }

    private void RetargetCameraToPlayer()
    {
        if (!cinemachineCam || !player) return;

        var t = cinemachineCam.Target;           // CM3: CameraTarget struct
        t.TrackingTarget = player.transform;     // follow the *new* player
        t.CustomLookAtTarget = false;
        cinemachineCam.Target = t;               // assign back
    }
}
