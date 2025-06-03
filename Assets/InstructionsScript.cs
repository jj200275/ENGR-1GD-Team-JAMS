using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class InstructionsScript : MonoBehaviour
{
    [Header("Panel to Control")]
    public CanvasGroup ADGroup;
    public CanvasGroup RGroup;
    public CanvasGroup SGroup;
    public CanvasGroup DubSpaceGroup;
    public CanvasGroup DubDGroup;

    [Header("Fade Settings")]
    public float fadeDuration = 5f;
    public float visibleDuration = 5f;

    public static bool ADhasFadedOut = false;
    public static bool RhasFadedOut = false;
    public static bool ShasFadedOut = false;
    public static bool DubSpacehasFadedOut = false;
    public static bool DubDhasFadedOut = false;
    public static bool ADwaitForExit = false;
    public static bool RwaitForExit = false;
    public static bool SwaitForExit = false;
    public static bool DubSpacewaitForExit = false;
    public static bool DubDwaitForExit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameObject.name.Contains("AD"))
            {
                // First trigger
                if (gameObject.name.Contains("Start"))
                {
                    StartCoroutine(FadeIn(ADGroup));
                }
                // Second trigger (exit point)
                else if (gameObject.name.Contains("End"))
                {
                    StartCoroutine(FadeOut(ADGroup));
                }
            }

            else if (gameObject.name.Contains("R"))
            {
                // First trigger
                if (gameObject.name.Contains("Start"))
                {
                    StartCoroutine(FadeIn(RGroup));
                }
                // Second trigger (exit point)
                else if (gameObject.name.Contains("End"))
                {
                    StartCoroutine(FadeOut(RGroup));
                }
            }
            if (gameObject.name.Contains("SKey"))
            {
                // First trigger
                if (gameObject.name.Contains("Start"))
                {
                    StartCoroutine(FadeIn(SGroup));
                }
                // Second trigger (exit point)
                else if (gameObject.name.Contains("End"))
                {
                    StartCoroutine(FadeOut(SGroup));
                }
            }
            if (gameObject.name.Contains("DubSpace"))
            {
                // First trigger
                if (gameObject.name.Contains("Start"))
                {
                    StartCoroutine(FadeIn(DubSpaceGroup));
                }
                // Second trigger (exit point)
                else if (gameObject.name.Contains("End"))
                {
                    StartCoroutine(FadeOut(DubSpaceGroup));
                }
            }
            if (gameObject.name.Contains("DubD"))
            {
                // First trigger
                if (gameObject.name.Contains("Start"))
                {
                    StartCoroutine(FadeIn(DubDGroup));
                }
                // Second trigger (exit point)
                else if (gameObject.name.Contains("End"))
                {
                    StartCoroutine(FadeOut(DubDGroup));
                }
            }
        }
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
        gameObject.SetActive(false);
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
        gameObject.SetActive(false);
    }
}