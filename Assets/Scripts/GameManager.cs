using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerControl.OnPlayerDied += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        PlayerControl.OnPlayerDied -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("GameManager: O Player morreu, respawnando...");

        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.RespawnPlayer();
        }
        else
        {
            Debug.LogWarning("Nenhum CheckpointManager encontrado. Reiniciando cena como fallback.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}
