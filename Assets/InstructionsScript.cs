using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class InstructionsScript : MonoBehaviour
{
    [Header("Panel to Control")]
    public CanvasGroup ADGroup;
    public CanvasGroup RGroup;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;
    public float visibleDuration = 5f;

    public static bool ADhasFadedOut = false;
    public static bool RhasFadedOut = false;
    public static bool ADwaitForExit = false;
    public static bool RwaitForExit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.name.Contains("AD"))
            {
                // First trigger
                if (!ADhasFadedOut && !ADwaitForExit)
                {
                    ADwaitForExit = true;
                    StartCoroutine(FadeSequence(ADGroup));
                }
                // Second trigger (exit point)
                else if (ADwaitForExit && !ADhasFadedOut)
                {
                    ADhasFadedOut = true;
                }
            }

            else if (gameObject.name.Contains("R"))
            {
                // First trigger
                if (!RhasFadedOut && !RwaitForExit)
                {
                    RwaitForExit = true;
                    StartCoroutine(FadeSequence(RGroup));
                }
                // Second trigger (exit point)
                else if (RwaitForExit && !RhasFadedOut)
                {
                    RhasFadedOut = true;
                }
            }
        }
    }

    private IEnumerator FadeSequence(CanvasGroup targetGroup)
    {
        yield return FadeIn(targetGroup);
        if (targetGroup.name.Contains("AD"))
        {
            yield return new WaitUntil(() => ADhasFadedOut);
            yield return FadeOut(targetGroup);
        }
        else if (targetGroup.name.Contains("R")) { }

    }

    private IEnumerator FadeIn(CanvasGroup group)
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(0, 1, time / fadeDuration);
            yield return null;
        }
        group.alpha = 1;
    }

    private IEnumerator FadeOut(CanvasGroup group)
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(1, 0, time / fadeDuration);
            yield return null;
        }
        group.alpha = 0;
    }
}