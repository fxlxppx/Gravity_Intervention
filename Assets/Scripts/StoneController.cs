using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PushableStone : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 initialPosition;

    [Header("Gravidade")]
    [SerializeField] private float normalGravity = 10f;
    [SerializeField] private float invertedGravity = -0.5f;

    private bool isGravityInverted = false;
    private bool playerInsideTrigger = false;

    [Header("Luz da Pedra")]
    [SerializeField] private Light2D stoneLight;
    [SerializeField] private float lightFadeSpeed = 3f;
    [SerializeField] private float targetIntensityOn = 3f;
    [SerializeField] private float targetIntensityOff = 0f;

    private float currentTargetIntensity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (stoneLight == null)
            stoneLight = GetComponentInChildren<Light2D>();
    }

    private void Start()
    {
        rb.gravityScale = normalGravity;
        initialPosition = transform.position;
        CheckpointManager.OnPlayerRespawn += ResetToInitialPosition;

        if (stoneLight != null)
            stoneLight.intensity = 0f;
    }

    private void OnDestroy()
    {
        CheckpointManager.OnPlayerRespawn -= ResetToInitialPosition;
    }

    private void Update()
    {
        if (playerInsideTrigger && PlayerControl.Instance != null)
        {
            bool playerGravityInverted = PlayerControl.Instance.IsGravityInverted();

            if (playerGravityInverted && !isGravityInverted)
            {
                ApplyInvertedGravity();
            }
            else if (!playerGravityInverted && isGravityInverted)
            {
                ResetGravity();
            }
        }

        if (stoneLight != null)
        {
            stoneLight.intensity = Mathf.Lerp(
                stoneLight.intensity,
                currentTargetIntensity,
                Time.deltaTime * lightFadeSpeed
            );
        }
    }

    private void ApplyInvertedGravity()
    {
        rb.gravityScale = invertedGravity;
        Vector3 scale = transform.localScale;
        scale.y = -Mathf.Abs(scale.y);
        transform.localScale = scale;
        isGravityInverted = true;
    }

    private void ResetGravity()
    {
        rb.gravityScale = normalGravity;
        Vector3 scale = transform.localScale;
        scale.y = Mathf.Abs(scale.y);
        transform.localScale = scale;
        isGravityInverted = false;
    }

    private void ResetToInitialPosition()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = initialPosition;

        ResetGravity();
        currentTargetIntensity = targetIntensityOff;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInsideTrigger = true;
            currentTargetIntensity = targetIntensityOn; // acende a luz
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInsideTrigger = false;
            currentTargetIntensity = targetIntensityOff; // apaga a luz
            ResetGravity();
        }
    }
}
