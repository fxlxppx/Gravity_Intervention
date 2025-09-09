using UnityEngine;

public class FinalDoor : MonoBehaviour
{
    public int leversRequired = 3; // n�mero de alavancas necess�rias

    void Update()
    {
        if (Lever.leversActivated >= leversRequired)
        {
            Debug.Log("Porta final aberta!");
            Destroy(gameObject);
        }
    }
}