using UnityEditor.Rendering;
using UnityEngine;
using Unity.Cinemachine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject startMenu;
    private CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 2f;
    [SerializeField] CinemachineCamera cineCam;
    [SerializeField] float startSize = 1.5f;
    [SerializeField] float endSize = 6f;
    [SerializeField] private GameObject overlay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startMenu.SetActive(true);
        Time.timeScale = 0;

        canvasGroup = startMenu.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void GameStart()
    {
        StartCoroutine(FadeOutAndStart());
    }

    private System.Collections.IEnumerator FadeOutAndStart()
    {
        float elapsed = 0f;
        Time.timeScale = 1;
        var lens = cineCam.Lens;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // use unscaled time because timeScale = 0
            float t = elapsed / fadeDuration;

            // Fade alpha
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            // Zoom out
            lens.OrthographicSize = Mathf.Lerp(startSize, endSize, t);
            cineCam.Lens = lens;

            // Re-bake the confiner with new camera size
            cineCam.GetComponent<CinemachineConfiner2D>().InvalidateBoundingShapeCache();

            yield return null;
        }

        canvasGroup.alpha = 0f;
        lens.OrthographicSize = endSize;
        cineCam.Lens = lens;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        startMenu.SetActive(false);
    }

    public void OverlayActivate()
    {
        overlay.SetActive(true);
    }

    public void OverlayDeactivate()
    {
        overlay.SetActive(false);
    }
}
