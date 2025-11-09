using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioDistance2D : MonoBehaviour
{
    public Transform listener;
    public float maxDistance = 10f;
    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
        if (listener == null)
            listener = Camera.main.transform;
    }

    void Update()
    {
        float dist = Vector2.Distance(transform.position, listener.position);
        source.volume = Mathf.Clamp01(1f - (dist / maxDistance));
    }
}
