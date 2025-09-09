using UnityEngine;

public class FinalDoor : MonoBehaviour
{
    public int leversRequired = 3; // número de alavancas necessárias

    void Update()
    {
        if (Lever.leversActivated >= leversRequired)
        {
            Debug.Log("Porta final aberta!");
            Destroy(gameObject);
        }
    }
}