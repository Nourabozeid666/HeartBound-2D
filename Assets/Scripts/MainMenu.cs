using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    const string StorySeenKey = "StorySeen";

    public void PlayerFirstScene()
    {
        bool storySeen = PlayerPrefs.GetInt(StorySeenKey, 0) == 1; // default 0 = not seen
        int sceneToLoad = storySeen ? 2 : 1;                       // 1 = story, 2 = gameplay
        SceneManager.LoadSceneAsync(sceneToLoad);                  // async load is fine. :contentReference[oaicite:0]{index=0}
    }

    public void Quit() => Application.Quit();
}
