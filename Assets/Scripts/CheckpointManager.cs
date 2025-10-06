using UnityEngine;
using UnityEngine.InputSystem;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }
    private Checkpoint currentCheckpoint;
    public static event System.Action OnPlayerRespawn;

    [SerializeField] private Checkpoint defaultCheckpoint;
    [SerializeField] private Transform playerTransform;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        PlayerControl.OnPlayerDied += RespawnPlayer;
    }

    private void OnDisable()
    {
        PlayerControl.OnPlayerDied -= RespawnPlayer;
    }

    private void Start()
    {
        currentCheckpoint = defaultCheckpoint;
    }

    public void SetCheckpoint(Checkpoint checkpoint)
    {
        if (currentCheckpoint == checkpoint) return;
        currentCheckpoint = checkpoint;
        Debug.Log("Novo checkpoint definido em: " + checkpoint.transform.position);
    }

    public void RespawnPlayer()
    {
        if (currentCheckpoint == null)
        {
            Debug.LogWarning("Nenhum checkpoint ativo!");
            return;
        }

        if (playerTransform != null)
        {
            OnPlayerRespawn?.Invoke();
            playerTransform.position = currentCheckpoint.GetRespawnPosition();
            Debug.Log("Respawn: Player movido para o checkpoint.");
        }
    }
}
