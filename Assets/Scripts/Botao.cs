using UnityEngine;

public class Botao : MonoBehaviour
{
    public Portinha portinha; // referência à portinha que ele abre

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Botão ativado!");
            if (portinha != null)
            {
                portinha.isOpen = true;
            }
            Destroy(gameObject); // botão desaparece após uso
        }
    }
}