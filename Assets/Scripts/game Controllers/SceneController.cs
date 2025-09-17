using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance = null;

    [Header("UI")]
    [SerializeField] GameObject gameOverPanel;
    //bool isDead = false;
    Animator anim;

    [Header("Random Level Pool (choose scene names you want to randomize)")]
    [Tooltip("Put scene numbers here (exactly as in Build Profiles Scene List).")]
    public List<string> levelSceneNames = new List<string>();

    [SerializeField] string mainMenuINT; 
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

    }
    public void NextLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        if (currentScene == 5)
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
        if (anim == null && gameOverPanel) 
            anim = gameOverPanel.GetComponent<Animator>();

        if (anim) anim.SetBool("isDead", true);

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
        if (anim) 
            anim.SetBool("isDead", false);
        // Unity will return a PlayerState even if its GameObject (or any parent) is disabled.
        var pi = FindFirstObjectByType<PlayerInput>(FindObjectsInactive.Include);
        if (pi) pi.enabled = true;
    }
}
