using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Patrulha")]
    public float moveSpeed = 3f;
    public Transform[] waypointObjects; // arraste aqui os filhos no Inspector
    private Vector2[] waypoints;        // cópia das posições fixas
    private int currentWaypoint = 0;

    [Header("Ataque")]
    public GameObject blobPrefab;
    public int blobsPerAttack = 5;
    public float attackCooldown = 2f;
    public float blobForce = 8f;

    private bool isAttacking = false;

    private void Start()
    {
        // copia as posições dos waypoints em espaço de mundo
        waypoints = new Vector2[waypointObjects.Length];
        for (int i = 0; i < waypointObjects.Length; i++)
        {
            waypoints[i] = waypointObjects[i].position;
        }
    }

    private void Update()
    {
        if (!isAttacking && waypoints.Length > 0)
        {
            MoveToWaypoint();
        }
    }

    private void MoveToWaypoint()
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 target = waypoints[currentWaypoint];

        transform.position = Vector2.MoveTowards(currentPos, target, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(currentPos, target) < 0.2f) // margem menor
        {
            StartCoroutine(AttackAtWaypoint());
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }

    private IEnumerator AttackAtWaypoint()
    {
        isAttacking = true;
        Debug.Log("Boss atacando!");

        for (int i = 0; i < blobsPerAttack; i++)
        {
            ShootBlob();
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private void ShootBlob()
    {
        Vector3 spawnPos = transform.position + Vector3.up * 1f; // 1 unidade acima do Boss
        GameObject blob = Instantiate(blobPrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb = blob.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
            rb.AddForce(direction * blobForce, ForceMode2D.Impulse);
        }
    }

}
