using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance = null;

    [Header("UI")]
    [SerializeField] GameObject gameOverPanel;
    //bool isDead = false;
    Animator gameOverPanalAnim;
    Animator playerAnim; 

    [Header("Random Level Pool (choose scene names you want to randomize)")]
    [Tooltip("Put scene numbers here (exactly as in Build Profiles Scene List).")]
    public List<string> levelSceneNames = new List<string>();

    [SerializeField] string mainMenuINT;

    // using Enemy Spawner 

    [Header("Enemy Spawner")]
    [SerializeField] int BaseAmount = 4;
    [SerializeField] int enemiesInc = 2;
    int LvlNum = 1;
    bool CanExitLevel = false;
    EnemySpawner enemySpawner;
    bool isDead = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        if (gameOverPanel == null)
            gameOverPanel = GameObject.FindWithTag("GameOverPanel");
        SceneManager.sceneLoaded+= OnSceneLoaded;

    }
     void OnDestroy()
    {
        if(instance == this)
            SceneManager.sceneLoaded-= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene , LoadSceneMode mode)
    {
        SetupLevel();
    }
    void SetupLevel()
    {
        CanExitLevel = false;
        HideGameOver();

        enemySpawner = FindFirstObjectByType<EnemySpawner>(FindObjectsInactive.Exclude);
        if(enemySpawner != null)
        {
            enemySpawner.OnBatchCleared -= HandleBatchCleared;
            enemySpawner.OnBatchCleared += HandleBatchCleared;

            int NumOfEnem = BaseAmount + (LvlNum - 1) * enemiesInc;
            enemySpawner.BeginBatch(NumOfEnem);
        }
        else
        {
            Debug.Log("No Enemyspawner");
        }
        gate Gate = FindFirstObjectByType<gate>();
        if (!Gate.BypassesLock) 
            Gate.SetLocked(true);
        else
            Gate.SetLocked(false);
    }
    void HandleBatchCleared(int BatchSize)
    {
        CanExitLevel = true;

        gate Gate = FindFirstObjectByType<gate>();
        if (!Gate.BypassesLock)
            Gate.SetLocked(false);
    }

    public bool canExitLevel => CanExitLevel;

    public void GoToNextLevel()
    {
        if (!CanExitLevel) return;
        LvlNum++;
        NextLevel();
    }
    public void NextLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        if (isDead)
        {
            //------------------------------------------------------------------
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            playerAnim = playerGO.GetComponent<Animator>();
            playerAnim.SetBool("isDead", false);
            SceneManager.LoadScene(0);
            isDead = false;
        }
        else if (currentScene == 5)
        {
            SceneManager.LoadScene(0);
        }
        else if (currentScene == 0)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            int i = Random.Range(0, levelSceneNames.Count);
            string nextScene = levelSceneNames[i];
            SceneManager.LoadScene(nextScene);
            levelSceneNames.RemoveAt(i);
            if(levelSceneNames.Count == 0)
            {
                SceneManager.LoadScene(0);
            }
        }
    }
    public void GameOver()
    {
        var pi = FindFirstObjectByType<PlayerInput>();
        if (pi) pi.enabled = false;

        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (gameOverPanalAnim == null && gameOverPanel) 
            gameOverPanalAnim = gameOverPanel.GetComponent<Animator>();

        if (gameOverPanalAnim) gameOverPanalAnim.SetBool("isDead", true);

        Invoke(nameof(EndGameOver),2);
        // SceneManager.LoadScene(0);
    }
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuINT);
    }
    public void HideGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
    void EndGameOver()
    {
        if (gameOverPanalAnim) 
            gameOverPanalAnim.SetBool("isDead", false);
        // Unity will return a PlayerState even if its GameObject (or any parent) is disabled.
        var pi = FindFirstObjectByType<PlayerInput>(FindObjectsInactive.Include);
        if (pi) pi.enabled = true;
        isDead = true;
        NextLevel();
    }
}
