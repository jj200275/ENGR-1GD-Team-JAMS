using UnityEngine;

public class SwitchDimensions : MonoBehaviour
{
    [SerializeField] GameObject presentDimension;
    [SerializeField] GameObject pastDimension;
    public bool present = true;
    public float timer;
    public float cooldown;
    void Start()
    {
        timer = 0;
        cooldown = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= 0) timer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.S) && timer <= 0)
        {
            switchDimension(present);
            timer = cooldown;
        }
    }

    void switchDimension(bool current)
    {
        if (current)
        {
            presentDimension.SetActive(false);
            pastDimension.SetActive(true);
            present = false;
        }
        else
        {
            presentDimension.SetActive(true);
            pastDimension.SetActive(false);
            present = true;
        }
    }
}
