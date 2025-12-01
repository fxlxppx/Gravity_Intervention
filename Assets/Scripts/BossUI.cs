using UnityEngine;
using UnityEngine.UI;

public class BossHealthDisplay : MonoBehaviour
{
    [Header("Sprites de cada estágio de vida (do cheio ao vazio)")]
    public Sprite[] healthStates;

    [Header("Renderer que mostra o sprite atual")]
    [SerializeField] private Image image;

    private int maxHealth = 6;
    private int currentHealth;

    private void OnEnable()
    {
        BossController.OnBossDamaged += OnBossDamaged;
    }

    private void OnDisable()
    {
        BossController.OnBossDamaged -= OnBossDamaged;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateSprite(currentHealth);
    }

    private void OnBossDamaged(BossController boss)
    {
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        currentHealth -= 1;

        UpdateSprite(currentHealth);
    }

    public void ResetHealth(int max)
    {
        maxHealth = max;
        currentHealth = maxHealth;
        UpdateSprite(currentHealth);
    }

    private void UpdateSprite(int current)
    {
        if (healthStates == null || healthStates.Length == 0) return;
        if (image == null) return;

        if (current >= 0 && current < healthStates.Length)
        {
            image.sprite = healthStates[current];
            Debug.Log("Updated boss health sprite to state " + current);
        }
    }
}
