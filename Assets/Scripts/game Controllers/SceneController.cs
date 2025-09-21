using System.Collections.Generic;
using UnityEngine;
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
       ;
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
            Gate.HandleGateOpened();
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
}
