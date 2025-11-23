using System;
using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public static event Action<BossController> OnBossDamaged;
    public static event Action<BossController> OnBossDeath;

    [Header("Patrulha")]
    public float moveSpeed = 3f;
    public Transform[] waypointObjects;
    private Vector2[] waypoints;
    private int currentWaypoint = 0;

    [Header("Ataque")]
    public GameObject blobPrefab;
    public int blobsPerAttack = 5;
    public float attackCooldown = 2f;
    public float blobForce = 8f;

    private bool isAttacking = false;

    [Header("Vida do Boss")]
    public int maxHealth = 6;
    private int currentHealth;

    [Header("Referências")]
    [SerializeField] private Animator animator;
    [SerializeField] private BossHealthDisplay bossHealthDisplay;
    [SerializeField] private GameObject bossHUD;

    [Header("Áudio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip damageSound;

    private void Start()
    {
        waypoints = new Vector2[waypointObjects.Length];
        for (int i = 0; i < waypointObjects.Length; i++)
            waypoints[i] = waypointObjects[i].position;

        currentHealth = maxHealth;

        if (animator != null)
            animator.Play("EnemyIdle");

        PlayIdleSound();
    }
    private void OnEnable()
    {
        CheckpointManager.OnPlayerRespawn += ResetBossHealth;
    }

    private void OnDisable()
    {
        CheckpointManager.OnPlayerRespawn -= ResetBossHealth;
    }

    private void Update()
    {
        if (!isAttacking && waypoints.Length > 0)
            MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 target = waypoints[currentWaypoint];
        transform.position = Vector2.MoveTowards(currentPos, target, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(currentPos, target) < 0.2f)
        {
            StartCoroutine(AttackAtWaypoint());
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }

    private IEnumerator AttackAtWaypoint()
    {
        isAttacking = true;
        StopIdleSound();

        if (animator != null)
            animator.SetTrigger("Attack");

        PlaySound(attackSound, 1f, 1f);
        yield return StartCoroutine(WaitForAnimationTime("EnemyAttack", 0.5f));

        for (int i = 0; i < blobsPerAttack; i++)
            ShootBlob();

        yield return new WaitForSeconds(attackCooldown);
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Idle");

        isAttacking = false;
        PlayIdleSound();
    }

    private IEnumerator WaitForAnimationTime(string animationName, float normalizedTime)
    {
        if (animator == null) yield break;
        yield return null;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            yield return null;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < normalizedTime)
            yield return null;
    }

    private void ShootBlob()
    {
        Vector3 spawnPos = transform.position + Vector3.up * 1f;
        GameObject blob = Instantiate(blobPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D rb = blob.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = new Vector2(UnityEngine.Random.Range(-1f, 1f), 1f).normalized;
            rb.AddForce(direction * blobForce, ForceMode2D.Impulse);
        }
    }

    private void ResetBossHealth()
    {
        currentHealth = maxHealth;

        if (bossHealthDisplay != null)
            bossHealthDisplay.ResetHealth(maxHealth);

        if (animator != null)
            animator.SetTrigger("Idle");
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (animator != null)
            animator.SetTrigger("Damage");
        PlaySound(damageSound, 1f, 1f);
        StartCoroutine(DamageRoutine());
        OnBossDamaged?.Invoke(this);

        if (currentHealth <= 0)
            StartCoroutine(DieSequence());
    }

    private IEnumerator DamageRoutine()
    {
        bool wasAttacking = isAttacking;
        isAttacking = true;
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyDamage"));
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
        animator.ResetTrigger("Damage");
        animator.SetTrigger("Idle");
        isAttacking = wasAttacking && currentHealth > 0 ? false : false;
    }

    private IEnumerator DieSequence()
    {
        StopIdleSound();
        PlaySound(deathSound, 1f, 1f);
        if (animator != null)
            animator.SetTrigger("Death");

        yield return new WaitForSeconds(1.2f);
        Die();
    }

    private void Die()
    {
        OnBossDeath?.Invoke(this);
        ButtonSystem.ReportButtonState(DoorColorEnum.Black, true);
        Destroy(gameObject);
        bossHUD.SetActive(false);
    }

    private void PlayIdleSound()
    {
        if (audioSource == null || idleSound == null) return;
        audioSource.clip = idleSound;
        audioSource.pitch = 0.2f;
        audioSource.volume = 0.7f;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void StopIdleSound()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    private void PlaySound(AudioClip clip, float volume, float pitch)
    {
        if (audioSource == null || clip == null) return;
        audioSource.Stop();
        audioSource.loop = false;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();
    }
}
