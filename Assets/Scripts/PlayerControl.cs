using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance { get; private set; }
    public static event System.Action OnPlayerDied;
    public static event System.Action<float> OnGravityInverted;
    public static event System.Action OnGravityReset;
    public static event System.Action OnGravityReady;

    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    private InputSystem_Actions controls;

    private float moveInput;

    [Header("Gravidade")]
    [SerializeField] private float normalGravity = 10f;
    [SerializeField] private float invertedGravity = -0.5f;
    [SerializeField] private float invertedGravityTime = 3f;
    [SerializeField] private float gravityCooldown = 5f;
    [SerializeField] private Light2D gravityIndicatorLight;
    [SerializeField] private float lightIncreaseDuration = 0.5f;

    private bool isGravityInverted = false;
    private float gravityTimer = 0f;
    private float cooldownTimer = 0f;
    private bool isDead = false;

    [Header("Vida do Player")]
    [SerializeField] private int maxLives = 3;
    private int currentLives;
    [SerializeField] private UIHearts uiHearts;

    [Header("Invulnerabilidade")]
    [SerializeField] private float invulnerabilityDuration = 2f;
    [SerializeField] private float blinkInterval = 0.2f;
    [SerializeField] private bool isInvulnerable = false;

    [SerializeField] private GameObject bossHUD;

    [Header("Áudio")]
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioClip gravityFlipSound;
    [SerializeField] private AudioClip gravityResetSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Som de Passos")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private float minMoveSpeed = 0.1f;

    private int collisionCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        rb = GetComponent<Rigidbody2D>();
        controls = new InputSystem_Actions();

        controls.Player.move.performed += ctx => moveInput = ctx.ReadValue<float>();
        controls.Player.move.canceled += ctx => moveInput = 0f;
        controls.Player.flip_gravity.performed += ctx => FlipGravity();
    }

    void OnEnable()
    {
        controls.Enable();
        CheckpointManager.OnPlayerRespawn += ResetGravity;
        CheckpointManager.OnPlayerRespawn += ResetPlayer;
    }

    void Start()
    {
        rb.gravityScale = normalGravity;
        currentLives = maxLives;

        if (uiHearts != null)
            uiHearts.UpdateHearts(currentLives);
    }

    void FixedUpdate()
    {
        if (isDead) return;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0.1f)
            spriteRenderer.flipX = false;
        else if (moveInput < -0.1f)
            spriteRenderer.flipX = true;

        if (animator != null)
            animator.SetFloat("Speed", Mathf.Abs(moveInput));

        UpdateFootstepSound();

        if (isGravityInverted)
        {
            gravityTimer -= Time.fixedDeltaTime;
            if (gravityTimer <= 0)
                ResetGravity();
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.fixedDeltaTime;
            if (cooldownTimer <= 0)
            {
                cooldownTimer = 0;
                OnGravityReady?.Invoke();
            }
        }
    }

    private void UpdateFootstepSound()
    {
        if (footstepSource == null) return;

        bool isMoving = Mathf.Abs(moveInput) > minMoveSpeed;
        bool isGrounded = collisionCount > 0;

        if (isMoving && isGrounded)
        {
            if (!footstepSource.isPlaying)
                footstepSource.Play();
        }
        else
        {
            if (footstepSource.isPlaying)
                footstepSource.Pause();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionCount++;
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Blobs") || collision.gameObject.CompareTag("Boss"))
        {
            if (!isInvulnerable)
                TakeDamage();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collisionCount--;
        if (collisionCount < 0) collisionCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Blobs") || collision.gameObject.CompareTag("Boss"))
        {
            if (!isInvulnerable)
                TakeDamage();
        }

        if (collision.gameObject.CompareTag("FOV"))
        {
            CameraFollow.Instance.TriggerBossFocus(8, 2f);
            bossHUD.SetActive(true);
            collision.gameObject.SetActive(false);
        }
    }

    private void FlipGravity()
    {
        if (cooldownTimer > 0f || isDead) return;

        if (!isGravityInverted)
        {
            if (animator != null)
                animator.SetTrigger("GravitySwap");

            rb.gravityScale = invertedGravity;
            StartCoroutine(IncreaseLightIntensity(0f, 3f, lightIncreaseDuration));

            Vector3 scale = transform.localScale;
            scale.y *= -1;
            transform.localScale = scale;

            isGravityInverted = true;
            gravityTimer = invertedGravityTime;
            OnGravityInverted?.Invoke(invertedGravityTime);
            CameraFollow.Instance.Shake(0.5f, 0.02f);

            PlaySound(gravityFlipSound, 0.3f, 1f);

            if (CooldownUI.Instance != null)
                CooldownUI.Instance.StartCooldown(gravityCooldown);
        }
        else
        {
            ResetGravity();
        }

        cooldownTimer = gravityCooldown;
    }

    private void ResetGravity()
    {
        if (isDead) return;

        rb.gravityScale = normalGravity;

        Vector3 scale = transform.localScale;
        scale.y = Mathf.Abs(scale.y);
        transform.localScale = scale;
        StartCoroutine(IncreaseLightIntensity(3f, 0f, lightIncreaseDuration));

        isGravityInverted = false;
        gravityTimer = 0f;
        OnGravityReset?.Invoke();
        CameraFollow.Instance.Shake(0.1f, 0.2f);

        PlaySound(gravityResetSound, 0.1f, 1f);

        if (CooldownUI.Instance != null)
            CooldownUI.Instance.FinishCooldown();

        if (cooldownTimer <= 0f)
            OnGravityReady?.Invoke();
    }

    public bool IsGravityInverted() => isGravityInverted;

    private void ResetPlayer()
    {
        gameObject.SetActive(true);
        currentLives = maxLives;
        isInvulnerable = false;
        isDead = false;

        if (animator != null)
        {
            animator.ResetTrigger("Death");
            animator.Play("Idle", 0, 0f);
        }

        if (uiHearts != null)
            uiHearts.UpdateHearts(currentLives);
    }

    private void TakeDamage()
    {
        if (isDead) return;

        currentLives--;
        Debug.Log("Player tomou dano! Vidas restantes: " + currentLives);
        CameraFollow.Instance.Shake(0.2f, 0.1f);

        PlaySound(damageSound, Random.Range(0.9f, 1.1f));

        if (uiHearts != null)
            uiHearts.UpdateHearts(currentLives);

        if (currentLives <= 0)
            Die();
        else
            StartCoroutine(InvulnerabilityRoutine());
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player morreu sem vidas!");

        PlaySound(deathSound, 1.0f);

        if (animator != null)
            animator.SetTrigger("Death");

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.8f);
        gameObject.SetActive(false);
        OnPlayerDied?.Invoke();
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        int originalLayer = LayerMask.NameToLayer("Default");
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");

        float elapsed = 0f;
        while (elapsed < invulnerabilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        spriteRenderer.enabled = true;
        isInvulnerable = false;
        gameObject.layer = originalLayer;
    }

    private IEnumerator IncreaseLightIntensity(float start, float end, float duration)
    {
        if (gravityIndicatorLight == null) yield break;

        float elapsed = 0f;
        gravityIndicatorLight.intensity = start;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            gravityIndicatorLight.intensity = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        gravityIndicatorLight.intensity = end;
    }

    private void PlaySound(AudioClip clip, float volume, float pitch = 1f)
    {
        if (playerAudioSource == null || clip == null) return;
        playerAudioSource.Stop();
        playerAudioSource.volume = volume;
        playerAudioSource.pitch = pitch;
        playerAudioSource.clip = clip;
        playerAudioSource.Play();
    }
}
