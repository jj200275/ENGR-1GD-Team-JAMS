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
    public float fadeDuration = 1f;
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
            if (gameObject.name.Contains("SKey"))
            {
                // First trigger
                if (!ShasFadedOut && !SwaitForExit)
                {
                    SwaitForExit = true;
                    StartCoroutine(FadeSequence(SGroup));
                }
                // Second trigger (exit point)
                else if (SwaitForExit && !ShasFadedOut)
                {
                    ShasFadedOut = true;
                }
            }
            if (gameObject.name.Contains("DubSpace"))
            {
                // First trigger
                if (!DubSpacehasFadedOut && !DubSpacewaitForExit)
                {
                    DubSpacewaitForExit = true;
                    StartCoroutine(FadeSequence(DubSpaceGroup));
                }
                // Second trigger (exit point)
                else if (DubSpacewaitForExit && !DubSpacehasFadedOut)
                {
                    DubSpacehasFadedOut = true;
                }
            }
            if (gameObject.name.Contains("DubD"))
            {
                // First trigger
                if (!DubDhasFadedOut && !DubDwaitForExit)
                {
                    DubDwaitForExit = true;
                    StartCoroutine(FadeSequence(DubDGroup));
                }
                // Second trigger (exit point)
                else if (DubDwaitForExit && !DubDhasFadedOut)
                {
                    DubDhasFadedOut = true;
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
        else if (targetGroup.name.Contains("SKey"))
        {
            yield return new WaitUntil(() => ShasFadedOut);
            yield return FadeOut(targetGroup);
        }
        else if (targetGroup.name.Contains("DubSpace"))
        {
            yield return new WaitUntil(() => DubSpacehasFadedOut);
            yield return FadeOut(targetGroup);
        }
        else if (targetGroup.name.Contains("DubD"))
        {
            yield return new WaitUntil(() => DubDhasFadedOut);
            yield return FadeOut(targetGroup);
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