using UnityEngine;

public class FallingRock : MonoBehaviour
{
    [Header("Configuração da Pedra")]
    public int damage = 1;
    public float lifetime = 10f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            Debug.Log("Boss atingido pela pedra!");
            BossController boss = collision.gameObject.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
