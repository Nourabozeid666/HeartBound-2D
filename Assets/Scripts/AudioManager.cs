using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    //a ‘static’ variable only one of it exists across all of object instances made of the class
    // it means the variable belongs to the class itself. It’s therefore shared by ALL object instances made of the class
    public static AudioManager instance = null;
    [SerializeField] AudioSource PlayerSounds;
    [SerializeField] AudioSource enemy_1;
    [SerializeField] AudioSource enemy_2;
    [SerializeField] AudioSource MusicInEnvironment;
    [SerializeField] float lowPitchRange;
    [SerializeField] float highPitchRange;

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
        DontDestroyOnLoad(gameObject);
    }
    //Plays a single specific sound effect immediately
    //You pass an AudioClip (like a footstep, hit, or click sound).
    public void PlaySingle(AudioClip clip)
    {
        PlayerSounds.clip = clip;
        PlayerSounds.Play();
    }
    // Plays a random clip from a list, with a random pitch variation — useful for making 
    // repeated sounds (like footsteps, hits, or gunshots) sound more natural and varied.
    // Use cases:
    //   Footsteps(so every step sounds slightly different)
    //   Gunfire(to avoid sounding too repetitive)
    //   Monster growls, slashes, and environmental sounds(to add realism)

    // params is a keyword in C# that allows you to pass a variable number 
    // of arguments to a method — like a flexible array.
    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        PlayerSounds.pitch = randomPitch;
        PlayerSounds.clip = clips[randomIndex];
        PlayerSounds.Play();
    }
    public void PlayerAction(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        PlayerSounds.pitch = randomPitch;
        PlayerSounds.clip = clips[randomIndex];
        PlayerSounds.Play();
    }
    public void EnemyAction(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        enemy_2.pitch = randomPitch;
        enemy_2.clip = clips[randomIndex];
        enemy_2.Play();
    }
    public void ChangeMusic(AudioClip newClip)
    {
        if (MusicInEnvironment.clip == newClip) return; // No need to replay if already playing!
        MusicInEnvironment.Stop();
        MusicInEnvironment.clip = newClip;
        MusicInEnvironment.Play();
    }


}