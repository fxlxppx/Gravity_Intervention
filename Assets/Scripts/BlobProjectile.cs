using UnityEngine;

public class BlobProjectile : MonoBehaviour
{
    [Header("Ricochetes")]
    public int maxBounces = 3;
    private int bounceCount = 0;

    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss != null)
        {
            Collider2D bossCol = boss.GetComponent<Collider2D>();
            if (bossCol != null && col != null)
            {
                Physics2D.IgnoreCollision(bossCol, col);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss")) return;
        if (collision.gameObject.CompareTag("Blobs")) return;

        bounceCount++;
        Debug.Log("Blob ricocheteou! " + bounceCount);

        if (bounceCount >= maxBounces)
        {
            Destroy(gameObject);
        }
    }
}
