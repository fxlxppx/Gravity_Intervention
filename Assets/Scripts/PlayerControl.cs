using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance { get; private set; }

    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private InputSystem_Actions controls;

    private float moveInput;

    [Header("Gravidade")]
    [SerializeField] private float normalGravity = 10f;       // mais forte para baixo
    [SerializeField] private float invertedGravity = -0.5f;   // menos forte para cima
    [SerializeField] private float invertedGravityTime = 3f;  // tempo que fica invertida
    [SerializeField] private float gravityCooldown = 5f;      // cooldown entre flips

    private bool isGravityInverted = false;
    private float gravityTimer = 0f;
    private float cooldownTimer = 0f;

    void Awake()
    {
        // Singleton para ser acessado pela pedra
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
        // aplica gravidade inicial
        rb.gravityScale = normalGravity;
    }

    void FixedUpdate()
    {
        // Movimento horizontal
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Flip do sprite
        if (moveInput > 0.1f)
            spriteRenderer.flipX = false;
        else if (moveInput < -0.1f)
            spriteRenderer.flipX = true;

        // Timer da gravidade invertida
        if (isGravityInverted)
        {
            gravityTimer -= Time.fixedDeltaTime;
            if (gravityTimer <= 0)
            {
                ResetGravity();
            }
        }

        // Atualiza cooldown
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.fixedDeltaTime;
        }
    }

    private void FlipGravity()
    {
        // se ainda está em cooldown, não faz nada
        if (cooldownTimer > 0f) return;

        if (!isGravityInverted)
        {
            // Ativa gravidade invertida (para cima)
            rb.gravityScale = invertedGravity;

            Vector3 scale = transform.localScale;
            scale.y *= -1;
            transform.localScale = scale;

            isGravityInverted = true;
            gravityTimer = invertedGravityTime;
        }
        else
        {
            // Se já está invertido e apertar de novo → volta ao normal antes do tempo
            ResetGravity();
        }

        // inicia cooldown
        cooldownTimer = gravityCooldown;
    }

    private void ResetGravity()
    {
        rb.gravityScale = normalGravity; // força maior para baixo

        Vector3 scale = transform.localScale;
        scale.y = Mathf.Abs(scale.y); // garante positivo
        transform.localScale = scale;

        isGravityInverted = false;
        gravityTimer = 0f;
    }

    // Getter para a pedra acessar
    public bool IsGravityInverted()
    {
        return isGravityInverted;
    }
}
