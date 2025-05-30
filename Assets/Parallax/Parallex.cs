using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPos, length;
    public GameObject cam;
    public float parallexEffect; // Speed at which background moves relative to camera

    void Start()
    {
        startPos = transform.position.x;
        if (TryGetComponent<SpriteRenderer>(out var sr))
        {
            length = sr.bounds.size.x;
        }
        else if (TryGetComponent<ParticleSystemRenderer>(out var psr))
        {
            length = psr.bounds.size.x;
        }
    }

    void FixedUpdate()
    {
        float distance = cam.transform.position.x * parallexEffect;
        float movement = cam.transform.position.x * (1 - parallexEffect);
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if (movement > startPos + length)
        {
            startPos += length;
        }
        else if (movement < startPos - length)
        {
            startPos -= length;
        }

    }
}
