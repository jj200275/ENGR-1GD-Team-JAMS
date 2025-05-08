using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject PauseMenuUI;
    public GameObject mainPanel;
    public GameObject audioPanel;
    public GameObject settingsPanel;
    public bool isPaused = false;
    private bool isAudio = false;
    [SerializeField] private Slider volumeSlider;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused && !isAudio)
            {
                ResumeGame();
            }
            else if (isPaused && isAudio)
            {
                BackToMain();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void ResumeGame()
    {
        PauseMenuUI.SetActive(false);
        settingsPanel.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        PauseMenuUI.SetActive(true);
        settingsPanel.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void OpenAudio()
    {
        mainPanel.SetActive(false);
        audioPanel.SetActive(true);
        isAudio = true;
    }

    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Application has quit.");
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }
    public void BackToMain()
    {
        mainPanel.SetActive(true);
        audioPanel.SetActive(false);
        isAudio = false;
    }
}
