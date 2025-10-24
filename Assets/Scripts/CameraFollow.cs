using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

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

    [Header("Shake Effect")]
    [SerializeField] private Vector3 originalPos;
    [SerializeField] private bool isShaking = false;
    private Vector3 shakeOffset = Vector3.zero;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

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

        transform.position = smoothedPosition + shakeOffset;
    }

    public void Shake(float duration, float magnitude)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            shakeOffset = new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        isShaking = false;
    }
}
