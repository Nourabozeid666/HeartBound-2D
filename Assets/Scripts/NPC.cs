using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, Iinteractable
{
    [Header("Data")]
    public DialogueSystem dialogueData;

    [Header("UI")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Image portraitImage;
    [SerializeField] Image Icon;

    int dialogueIndex = 0;
    bool isTyping = false, isDialogueActive = false;

    void Start() => StartDialogue();

    public void StartDialogue()
    {
        if (dialogueData == null || dialogueData.lines.Count == 0) return;

        isDialogueActive = true;
        dialogueIndex = 0;

        if (dialoguePanel) dialoguePanel.SetActive(true);
        if (nameText) nameText.gameObject.SetActive(true);
        if (portraitImage) portraitImage.gameObject.SetActive(true);

        StartCoroutine(TypeLine());
    }
    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;

        if (dialogueText) dialogueText.SetText("");
        if (dialoguePanel) dialoguePanel.SetActive(false);

        // Deactivate the name and sprite UI
        if (nameText) nameText.gameObject.SetActive(false);
        if (portraitImage) portraitImage.gameObject.SetActive(false);
        Icon.gameObject.SetActive(false);
    }
    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        var line = dialogueData.lines[dialogueIndex];

        // Set name
        if (nameText) nameText.SetText(line.characterName ?? "");

        // Set portrait
        if (portraitImage)
        {
            if (line.characterImage != null)
            {
                portraitImage.sprite = line.characterImage;
                portraitImage.enabled = true;
            }
            else
            {
                portraitImage.enabled = false;
            }
        }

        // (Optional) play voiceOver: one clip per line, if you wire an AudioSource
        // if (line.voiceOver) audioSource.PlayOneShot(line.voiceOver);

        // Type text
        foreach (char c in line.speech)
        {
            dialogueText.text += c;                       // SetText works fine for TMP
            yield return new WaitForSeconds(dialogueData.typeSpeed); // typed effect
        }
        isTyping = false;
    }

    public void NextLine()
    {
        if (!isDialogueActive) return;

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.lines[dialogueIndex].speech);
            isTyping = false;
            return;
        }

        dialogueIndex++;
        if (dialogueIndex < dialogueData.lines.Count)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }



    // Iinteractable
    void Iinteractable.interact()
    {
        if (dialogueData == null) return;

        if (isDialogueActive) NextLine();
        else StartDialogue();
    }

    bool Iinteractable.canInteract() => !isDialogueActive;
}
