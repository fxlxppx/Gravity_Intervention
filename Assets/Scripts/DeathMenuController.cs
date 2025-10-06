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

    public void Show()
    {
        if (deathMenuPanel != null)
            deathMenuPanel.SetActive(true);

        Time.timeScale = 0f;

        Debug.Log("DeathMenuUI: Menu de morte exibido. Jogo pausado.");
    }

    private void Hide()
    {
        if (deathMenuPanel != null)
            deathMenuPanel.SetActive(false);

        Time.timeScale = 1f;
    }

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

    public void OnReturnToMainMenu()
    {
        Debug.Log("DeathMenuUI: Retornando ao menu principal...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
