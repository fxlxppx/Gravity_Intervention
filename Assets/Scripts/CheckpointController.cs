using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Checkpoint ativado!");
            CheckpointManager.Instance.SetCheckpoint(this);
        }
    }

    public Vector3 GetRespawnPosition()
    {
        return transform.position;
    }
}
