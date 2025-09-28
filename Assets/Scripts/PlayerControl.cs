using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance { get; private set; }
    public static event System.Action OnPlayerDied;

    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private InputSystem_Actions controls;

    private float moveInput;

    [Header("Gravidade")]
    [SerializeField] private float normalGravity = 10f;
    [SerializeField] private float invertedGravity = -0.5f;
    [SerializeField] private float invertedGravityTime = 3f;
    [SerializeField] private float gravityCooldown = 5f;

    private bool isGravityInverted = false;
    private float gravityTimer = 0f;
    private float cooldownTimer = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        controls = new InputSystem_Actions();

        controls.Player.move.performed += ctx => moveInput = ctx.ReadValue<float>();
        controls.Player.move.canceled += ctx => moveInput = 0f;

        controls.Player.flip_gravity.performed += ctx => FlipGravity();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        rb.gravityScale = normalGravity;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0.1f)
            spriteRenderer.flipX = false;
        else if (moveInput < -0.1f)
            spriteRenderer.flipX = true;

        if (isGravityInverted)
        {
            gravityTimer -= Time.fixedDeltaTime;
            if (gravityTimer <= 0)
            {
                ResetGravity();
            }
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.fixedDeltaTime;
        }
    }

    private void FlipGravity()
    {
        if (cooldownTimer > 0f) return;

        if (!isGravityInverted)
        {
            rb.gravityScale = invertedGravity;

            Vector3 scale = transform.localScale;
            scale.y *= -1;
            transform.localScale = scale;

            isGravityInverted = true;
            gravityTimer = invertedGravityTime;

            if (CooldownUI.Instance != null)
            {
                CooldownUI.Instance.StartCooldown(gravityCooldown);
            }
        }
        else
        {
            ResetGravity();
        }
        cooldownTimer = gravityCooldown;
    }

    private void ResetGravity()
    {
        rb.gravityScale = normalGravity;

        Vector3 scale = transform.localScale;
        scale.y = Mathf.Abs(scale.y);
        transform.localScale = scale;

        isGravityInverted = false;
        gravityTimer = 0f;

        if (CooldownUI.Instance != null)
        {
            CooldownUI.Instance.FinishCooldown();
        }
    }
    public bool IsGravityInverted()
    {
        return isGravityInverted;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Blobs"))
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Blobs"))
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player morreu!");

        OnPlayerDied?.Invoke();

        Destroy(gameObject);
    }
}
