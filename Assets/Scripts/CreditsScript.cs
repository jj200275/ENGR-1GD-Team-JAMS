using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI thankyou;
    [SerializeField] Image title;
    [SerializeField] TextMeshProUGUI team;
    public float fade;
    public bool disappear;
    public int step;
    public float fademod;

    void Start()
    {
        fade = 0;
        step = 0;
        disappear = false;
        thankyou.color = new Color(1, 1, 1, 0);
        title.color = new Color(1, 1, 1, 0);
        team.color = new Color(1, 1, 1, 0);
    }

    void FixedUpdate()
    {
        switch (step)
        {
            case 0:
                if (!disappear)
                {
                    fade += fademod;
                    if(fade > 1)
                    {
                        disappear = true;
                    }
                }
                else
                {
                    fade -= fademod;
                    if(fade <= 0)
                    {
                        step = 1;
                        disappear = false;
                    }
                }
                thankyou.color = new Color(1, 1, 1, fade);
                break;
            case 1:
                if (!disappear)
                {
                    fade += fademod;
                    if (fade > 1)
                    {
                        disappear = true;
                    }
                }
                else
                {
                    fade -= fademod;
                    if (fade <= 0)
                    {
                        step = 2;
                        disappear = false;
                    }
                }
                title.color = new Color(1, 1, 1, fade);
                break;
            case 2:
                if (!disappear)
                {
                    fade += fademod;
                    if (fade > 1)
                    {
                        disappear = true;
                    }
                }
                team.color = new Color(1, 1, 1, fade);
                break;
            default:
                step = 0;
                break;
        }
    }
}
