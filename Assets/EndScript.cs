using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScript : MonoBehaviour
{
    [SerializeField] Image blackfade;
    public bool startEnd = false;
    public float fade = 1;

    void Start()
    {
        startEnd = false;
        fade = 0;
    }
    void FixedUpdate()
    {
        if (startEnd)
        {
            blackfade.color = new Color(0, 0, 0, fade);
            if (fade < 1)
            {
                fade += 0.01f;

            }
            else
            {
                SceneManager.LoadScene("Credits");  // CHANGE TO END SCENE ONCE GET! - Loads end scene
            }
        }
    }
}
