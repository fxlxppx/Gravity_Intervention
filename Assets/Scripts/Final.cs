using UnityEngine;

public class Final : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jogador chegou no final!");
            Application.Quit(); // fecha no jogo compilado
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // fecha dentro do editor
#endif
        }
    }
}