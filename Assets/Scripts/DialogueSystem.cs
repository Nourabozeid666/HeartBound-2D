using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue")]
public class DialogueSystem : ScriptableObject
{
    [System.Serializable]
    public class Line
    {
        public string characterName;
        [TextArea] public string speech;
        public Sprite characterImage;
        public AudioClip voiceOver; // optional, per-line
    }

    public List<Line> lines = new();
    public float typeSpeed = 0.02f;
}
