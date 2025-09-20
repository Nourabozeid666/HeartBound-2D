using UnityEngine;
[CreateAssetMenu(fileName = "Dialogue",menuName = "Dialogue")]
public class DialogueSystem : ScriptableObject
{
    public string[] characterName;
    public string[] speech;
    public Sprite[] characterImage;
    public float typeSpeed = 0.02f;
    public AudioClip voiceOver;
     
}
