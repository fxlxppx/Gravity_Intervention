using UnityEngine;
using UnityEngine.InputSystem;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }
    private Checkpoint currentCheckpoint;

    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("Respawnando jogador via tecla R");
            RespawnPlayer();
        }
    }


    public void SetCheckpoint(Checkpoint checkpoint)
    {
        currentCheckpoint = checkpoint;
    }

    public void RespawnPlayer()
    {
        if (currentCheckpoint == null)
        {
            Debug.LogWarning("Nenhum checkpoint ativo!");
            return;
        }

        Instantiate(playerPrefab, currentCheckpoint.GetRespawnPosition(), Quaternion.identity);
    }
}
