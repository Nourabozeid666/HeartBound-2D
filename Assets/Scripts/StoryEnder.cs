using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryEnder : MonoBehaviour
{
    const string StorySeenKey = "StorySeen";

    public void OnStoryFinished()
    {
        PlayerPrefs.SetInt(StorySeenKey, 1);
        PlayerPrefs.Save();                        
        SceneManager.LoadSceneAsync(2);           
    }
}
