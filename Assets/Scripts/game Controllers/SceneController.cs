using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;   // NEW
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance = null;

    [Header("UI")]
    [SerializeField] GameObject gameOverPanel;
    Animator gameOverAnim;

    [Header("Levels (names from Build Settings)")]
    public List<string> levelSceneNames = new List<string>();
    [SerializeField] string mainMenuINT;

    [Header("Enemy Progression")]
    [SerializeField] int baseAmount = 4;
    [SerializeField] int enemiesInc = 2;

    // NEW — tell the controller which UI root must be unique (tag it "HUD" in Inspector)
    [Header("Singleton Roots (by Tag)")]
    [SerializeField] string hudTag = "HUD";        // tag the *health & ability UI* root with this
    [SerializeField] string mainCameraTag = "MainCamera"; // default Main Camera tag

    int levelNum = 1;
    bool canExitLevel = false;
    bool isDeadFlow = false;

    List<EnemySpawner> spawners = new List<EnemySpawner>();
    int levelAlive = 0;

    // -------- Lifecycle --------
    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else if (instance != this) { Destroy(gameObject); return; }

        if (!gameOverPanel) gameOverPanel = GameObject.FindWithTag("GameOverPanel");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupLevel();

        // --- NEW: clean scene-level duplicates & rebind camera every time we load ---
        KeepOneByTag(hudTag);           // keep one HUD (health & ability UI)
        EnsureSingleEventSystem();      // one EventSystem only
        EnsureSingleAudioListener();    // one enabled AudioListener only
        RebindCinemachineToPlayer();    // point camera to the new Player
    }

    // -------- Level Setup / Flow --------
    void SetupLevel()
    {
        canExitLevel = false;
        levelAlive = 0;
        HideGameOver();

        foreach (var sp in spawners)
        {
            if (!sp) continue;
            sp.OnEnemySpawned -= HandleEnemySpawned;
            sp.OnEnemyDied -= HandleEnemyDied;
        }
        spawners.Clear();

        spawners.AddRange(FindObjectsByType<EnemySpawner>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None));
        foreach (var sp in spawners)
        {
            sp.OnEnemySpawned += HandleEnemySpawned;
            sp.OnEnemyDied += HandleEnemyDied;
        }

        int enemiesThisLevel = baseAmount + ((levelNum - 1) * enemiesInc);

        if (spawners.Count == 0)
        {
            Debug.LogWarning("[SceneController] No EnemySpawner found in this scene.");
        }
        else
        {
            int perSpawner = Mathf.Max(1, enemiesThisLevel / spawners.Count);
            int remainder = Mathf.Max(0, enemiesThisLevel - perSpawner * spawners.Count);

            for (int i = 0; i < spawners.Count; i++)
            {
                int count = perSpawner + (i < remainder ? 1 : 0);
                spawners[i].BeginBatch(count);
            }
        }
    }

    void HandleEnemySpawned()
    {
        levelAlive++;
    }

    void HandleEnemyDied()
    {
        levelAlive = Mathf.Max(0, levelAlive - 1);
        TryOpenGatesIfClear();
    }

    void TryOpenGatesIfClear()
    {
        if (levelAlive > 0) return;

        foreach (var sp in spawners)
            if (sp && sp.IsSpawning) return;

        canExitLevel = true;

        gate Gate = FindFirstObjectByType<gate>();

        foreach (var ga in FindObjectsByType<gateAnim>(
                     FindObjectsInactive.Include,
                     FindObjectsSortMode.None))
        {
            ga.Open();
            if (Gate) Gate.HandleGateOpened();
        }
    }

    public bool CanExitLevel => canExitLevel;

    public void GoToNextLevel()
    {
        if (!canExitLevel) return;
        levelNum++;
        NextLevel();
    }

    public void NextLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (isDeadFlow)
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            var playerAnim = playerGO ? playerGO.GetComponent<Animator>() : null;
            if (playerAnim) playerAnim.SetBool("isDead", false);

            SceneManager.LoadScene("the Hub Scene");
            isDeadFlow = false;
            return;
        }
        else if (currentScene == 2)
        {
            SceneManager.LoadScene(3);
        }
        else
        {
            if (levelSceneNames.Count > 0)
            {
                int i = Random.Range(0, levelSceneNames.Count);
                string nextScene = levelSceneNames[i];
                SceneManager.LoadScene(nextScene);
                levelSceneNames.RemoveAt(i);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    // -------- Game Over --------
    public void GameOver()
    {
        var pi = FindFirstObjectByType<PlayerInput>();
        if (pi) pi.enabled = false;

        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (!gameOverAnim && gameOverPanel)
            gameOverAnim = gameOverPanel.GetComponent<Animator>();

        if (gameOverAnim) gameOverAnim.SetBool("isDead", true);

        Invoke(nameof(EndGameOver), 2f);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuINT);
    }

    public void HideGameOver()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    void EndGameOver()
    {
        if (gameOverAnim) gameOverAnim.SetBool("isDead", false);
        var pi = FindFirstObjectByType<PlayerInput>(FindObjectsInactive.Include);
        if (pi) pi.enabled = true;
        isDeadFlow = true;
        NextLevel();
    }

    // ----------------- NEW HELPERS -----------------

    // Keep only one object that uses a specific Tag
    void KeepOneByTag(string tag)
    {
        if (string.IsNullOrEmpty(tag)) return;

        var objs = GameObject.FindGameObjectsWithTag(tag);
        if (objs == null || objs.Length <= 1) return;

        // Keep the first found; destroy the rest
        var keep = objs[0];
        for (int i = 1; i < objs.Length; i++)
        {
            if (objs[i] && objs[i] != keep)
                Destroy(objs[i]);
        }
    }

    // There must be exactly one active EventSystem in the scene
    void EnsureSingleEventSystem()
    {
        var systems = FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (systems.Length <= 1) return;

        // Keep the first enabled one (or first)
        EventSystem keep = null;
        foreach (var es in systems) { if (es && es.enabled) { keep = es; break; } }
        if (!keep) keep = systems[0];

        foreach (var es in systems)
            if (es != keep) Destroy(es.gameObject);
        // One EventSystem can drive all your canvases. :contentReference[oaicite:4]{index=4}
    }

    // Only one enabled AudioListener at a time
    void EnsureSingleAudioListener()
    {
        var listeners = FindObjectsByType<AudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (listeners.Length == 0) return;

        // Disable all, then re-enable the one on the MainCamera (if present)
        foreach (var l in listeners) if (l) l.enabled = false;

        var mainCam = GameObject.FindGameObjectWithTag(mainCameraTag);
        var keep = mainCam ? mainCam.GetComponent<AudioListener>() : null;

        if (keep) keep.enabled = true;
        else listeners[0].enabled = true; // fallback
        // Unity supports only one active AudioListener. :contentReference[oaicite:5]{index=5}
    }

    // Reconnect Cinemachine to the freshly spawned Player after load
    void RebindCinemachineToPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return;

        // Find all CM3 cameras (even if disabled)
        var cmCams = FindObjectsByType<CinemachineCamera>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var cam in cmCams)
        {
            if (!cam) continue;

            // IMPORTANT: Target is a struct — copy, modify, assign back
            var t = cam.Target;
            if (t.TrackingTarget == null)
            {
                t.TrackingTarget = player.transform;
                cam.Target = t;                       // write back the struct
            }
        }
    }
}

