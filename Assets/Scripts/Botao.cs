using UnityEngine;

public class Botao : MonoBehaviour
{
    public Portinha portinha; // refer�ncia � portinha que ele abre

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Bot�o ativado!");
            if (portinha != null)
            {
                portinha.isOpen = true;
            }
            Destroy(gameObject); // bot�o desaparece ap�s uso
        }
    }
}