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
        Debug.Log("GameManager: Player morreu — mostrando menu de morte.");

        if (DeathMenuUI.Instance != null)
        {
            DeathMenuUI.Instance.Show();
        }
        else
        {
            Debug.LogWarning("DeathMenuUI não encontrado. Reiniciando cena como fallback.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}
