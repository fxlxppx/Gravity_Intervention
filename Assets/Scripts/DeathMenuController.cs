using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenuUI : MonoBehaviour
{
    public static DeathMenuUI Instance { get; private set; }

    [SerializeField] private GameObject deathMenuPanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (deathMenuPanel != null)
            deathMenuPanel.SetActive(false);
    }

    /// <summary>
    /// Exibe o menu e pausa o jogo.
    /// </summary>
    public void Show()
    {
        if (deathMenuPanel != null)
            deathMenuPanel.SetActive(true);

        // Pausa o jogo — nada se move até o jogador escolher algo
        Time.timeScale = 0f;

        Debug.Log("DeathMenuUI: Menu de morte exibido. Jogo pausado.");
    }

    /// <summary>
    /// Oculta o menu e retoma o jogo.
    /// </summary>
    private void Hide()
    {
        if (deathMenuPanel != null)
            deathMenuPanel.SetActive(false);

        // Retoma o tempo do jogo
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Botão: Respawn no último checkpoint.
    /// </summary>
    public void OnRespawnButton()
    {
        Debug.Log("DeathMenuUI: Respawn selecionado.");
        Hide();

        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.RespawnPlayer();
        }
        else
        {
            Debug.LogWarning("Nenhum CheckpointManager encontrado. Reiniciando cena...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    /// <summary>
    /// Botão: Voltar ao menu principal.
    /// </summary>
    public void OnReturnToMainMenu()
    {
        Debug.Log("DeathMenuUI: Retornando ao menu principal...");
        Time.timeScale = 1f; // retoma o tempo antes de trocar de cena
        SceneManager.LoadScene("MainMenu");
    }
}
