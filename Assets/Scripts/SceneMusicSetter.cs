using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMusicSetter : MonoBehaviour
{
    [SerializeField] private AudioClip sceneMusic;

    void Start()
    {
        if (AudioManager.instance != null && sceneMusic != null)
        {
            AudioManager.instance.ChangeMusic(sceneMusic);
        }
    }
}