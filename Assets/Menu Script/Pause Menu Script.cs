using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Application has quit.");
    }
}
