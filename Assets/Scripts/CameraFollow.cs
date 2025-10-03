using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offsets")]
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Smoothness")]
    public float smoothSpeedX = 8f;
    public float smoothSpeedY = 2f;

    [Header("Pixel Perfect (opcional)")]
    public bool pixelSnap = false;
    public float snapValue = 0.01f;

    [Header("Fade Effect")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    void OnEnable()
    {
        PlayerControl.OnPlayerDied += HandlePlayerDeath;
    }

    void OnDisable()
    {
        PlayerControl.OnPlayerDied -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        yield return StartCoroutine(Fade(1f, 0f));

        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = c;
            yield return null;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        float newX = Mathf.Lerp(transform.position.x, desiredPosition.x, smoothSpeedX * Time.deltaTime);
        float newY = Mathf.Lerp(transform.position.y, desiredPosition.y, smoothSpeedY * Time.deltaTime);

        Vector3 smoothedPosition = new Vector3(newX, newY, desiredPosition.z);

        if (pixelSnap)
        {
            smoothedPosition.x = Mathf.Round(smoothedPosition.x / snapValue) * snapValue;
            smoothedPosition.y = Mathf.Round(smoothedPosition.y / snapValue) * snapValue;
        }

        transform.position = smoothedPosition;
    }
}
