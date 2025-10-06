using UnityEngine;

public class Spikes : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jogador tocou nos espinhos!");

            PlayerControl player = other.GetComponent<PlayerControl>();
            if (player != null)
            {
                player.SendMessage("TakeDamage");
            }
        }
    }
}
