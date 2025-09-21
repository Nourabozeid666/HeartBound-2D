using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;    // optional, for next scene
using TMPro;
using UnityEngine.UI;

public class StorySequenceController : MonoBehaviour
{
    [Header("Canvas Groups (roots)")]
    public CanvasGroup canvas1Intro;
    public CanvasGroup canvas2Dialogue;
    public CanvasGroup canvas3Interlude;
    public CanvasGroup canvas4Dialogue;

    [Header("Dialogue Controllers")]
    public NPC npc2;
    public NPC npc4;

    [Header("Timings")]
    public float fadeIn = 0.5f;
    public float holdIntro1 = 2.5f;
    public float holdIntro3 = 2.5f;
    public float fadeOut = 0.5f;

    [Header("After sequence")]
    public bool loadNextScene = false;
    public string nextSceneName = "GameScene"; // optional

    void Start()  => StartCoroutine(RunSequence()); 


    IEnumerator RunSequence()
    {
        // Make sure everything starts hidden/non-interactive
        InitGroup(canvas1Intro, 0, false);
        InitGroup(canvas2Dialogue, 0, false);
        InitGroup(canvas3Interlude, 0, false);
        InitGroup(canvas4Dialogue, 0, false);

        // 1) Intro: fade in, hold, fade out
        yield return FadeTo(canvas1Intro, 1f, fadeIn);
        yield return new WaitForSeconds(holdIntro1);              // standard coroutine wait :contentReference[oaicite:4]{index=4}
        yield return FadeTo(canvas1Intro, 0f, fadeOut);
        SetInteractive(canvas1Intro, false);

        // 2) Dialogue 1: show, run until finished, hide
        SetInteractive(canvas2Dialogue, true);
        yield return FadeTo(canvas2Dialogue, 1f, fadeIn);
        if (!npc2) npc2 = canvas2Dialogue.GetComponentInChildren<NPC>(true);
        npc2.StartDialogue();
        yield return new WaitUntil(() => !npc2.IsDialogueActive); // wait until dialogue ends :contentReference[oaicite:5]{index=5}
        yield return FadeTo(canvas2Dialogue, 0f, fadeOut);
        SetInteractive(canvas2Dialogue, false);

        // 3) Interlude: fade in, hold, fade out
        yield return FadeTo(canvas3Interlude, 1f, fadeIn);
        yield return new WaitForSeconds(holdIntro3);
        yield return FadeTo(canvas3Interlude, 0f, fadeOut);
        SetInteractive(canvas3Interlude, false);

        // 4) Dialogue 2: show, run until finished, hide
        SetInteractive(canvas4Dialogue, true);
        yield return FadeTo(canvas4Dialogue, 1f, fadeIn);
        if (!npc4) npc4 = canvas4Dialogue.GetComponentInChildren<NPC>(true);
        npc4.StartDialogue();
        yield return new WaitUntil(() => !npc4.IsDialogueActive);
        yield return FadeTo(canvas4Dialogue, 0f, fadeOut);
        SetInteractive(canvas4Dialogue, false);
        loadNextScene = true;

        // Optional: start the game / load scene
        if (loadNextScene )
            SceneManager.LoadScene("the Hub Scene");                // loads next frame; consider Async if you prefer :contentReference[oaicite:6]{index=6}
    }

    // --- helpers ---
    void InitGroup(CanvasGroup g, float alpha, bool interactive)
    {
        if (!g) return;
        g.alpha = alpha;
        g.gameObject.SetActive(true);
        SetInteractive(g, interactive);
    }

    void SetInteractive(CanvasGroup g, bool on)
    {
        if (!g) return;
        g.interactable = on;
        g.blocksRaycasts = on;     // blocks clicks only while active :contentReference[oaicite:7]{index=7}
    }

    IEnumerator FadeTo(CanvasGroup g, float target, float duration)
    {
        if (!g || duration <= 0f) { if (g) g.alpha = target; yield break; }
        float start = g.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            g.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;     // do it over multiple frames; see coroutine docs :contentReference[oaicite:8]{index=8}
        }
        g.alpha = target;
    }
}