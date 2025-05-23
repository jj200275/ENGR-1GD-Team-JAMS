using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour
{
    [SerializeField] Image vignette;
    public float fade;
    public bool present;
    void Start()
    {
        fade = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        vignette.color = new Color(1, 1, 1, fade);
        if (present)
        {
            if (fade < 1)
            {
                fade += 0.1f;
            }
        }
        else
        {
            if (fade > 0)
            {
                fade -= 0.1f;
            }
        }
    }
}
