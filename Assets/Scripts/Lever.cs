using UnityEngine;
using UnityEngine.InputSystem;

public class Lever : MonoBehaviour
{
    public static int leversActivated = 0;   // contador global
    private bool playerInRange = false;
    private InputSystem_Actions controls;

    void OnEnable()
    {
        controls = new InputSystem_Actions();
        controls.Player.Enable();
        controls.Player.Interact.performed += OnInteract;
    }

    void OnDisable()
    {
        controls.Player.Interact.performed -= OnInteract;
        controls.Player.Disable();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (playerInRange)
            ActivateLever();
    }

    void ActivateLever()
    {
        leversActivated++;
        Debug.Log("Alavanca ativada! Total: " + leversActivated);
        Destroy(gameObject); // faz a alavanca desaparecer
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}