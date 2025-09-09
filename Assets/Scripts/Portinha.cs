using UnityEngine;

public class Portinha : MonoBehaviour
{
    [HideInInspector] public bool isOpen = false; // vai ser ativada pelo Botao.cs

    void Update()
    {
        if (isOpen)
        {
            Debug.Log("Portinha aberta!");
            Destroy(gameObject); // a portinha desaparece
        }
    }
}