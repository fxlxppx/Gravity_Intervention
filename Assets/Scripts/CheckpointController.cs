using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private BoxCollider2D checkpointCollider;
    [SerializeField] private SpriteRenderer checkpointSprite;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CheckpointManager.Instance.SetCheckpoint(this);

            if (checkpointCollider != null)
                checkpointCollider.enabled = false;

            if (checkpointSprite != null)
                checkpointSprite.enabled = false;
        }
    }

    public Vector3 GetRespawnPosition()
    {
        return transform.position;
    }
}
