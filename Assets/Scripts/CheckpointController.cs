using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private BoxCollider2D checkpointCollider;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CheckpointManager.Instance.SetCheckpoint(this);
            if (checkpointCollider != null)
                checkpointCollider.enabled = false;
        }
    }

    public Vector3 GetRespawnPosition()
    {
        return transform.position;
    }
}
