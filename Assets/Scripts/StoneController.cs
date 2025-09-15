using UnityEngine;

public class PushableStone : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Gravidade")]
    [SerializeField] private float normalGravity = 10f;
    [SerializeField] private float invertedGravity = -0.5f;

    private bool isGravityInverted = false;
    private bool playerInsideTrigger = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.gravityScale = normalGravity;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInsideTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInsideTrigger = false;
            ResetGravity();
        }
    }
}
