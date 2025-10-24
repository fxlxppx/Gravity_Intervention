using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject pauseMenuUI; // painel do menu de pause

    private InputSystem_Actions controls;

    private void Awake()
    {
        controls = new InputSystem_Actions();

        controls.UI.Pause.performed += ctx => TogglePause();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.UI.Enable();
    }

    private void OnDisable() => controls.Disable();

    public void TogglePause()
    {
        Console.WriteLine("TogglePause called");
        if (IsPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void RespawnAtCheckpoint()
    {
        if (CheckpointManager.Instance != null)
        {
            Console.WriteLine("RespawnAtCheckpoint called");
            CheckpointManager.Instance.RespawnPlayer();
            Resume();
        }
        else
            Debug.LogWarning("CheckpointManager não encontrado!");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
