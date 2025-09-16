using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class ButtonController2D : MonoBehaviour
{
    [Header("Config")]
    public DoorColorEnum color = DoorColorEnum.Red;
    [Tooltip("Layers que podem ativar o botão (Player, Stone etc.)")]
    public LayerMask activatorLayers;
    [Tooltip("Se true o botão permanece pressionado até o objeto sair; se false, botão é momentâneo (não recomendado para pushable stones)")]
    public bool holdWhileObjectOnTop = true;

    [Header("Visual / Feedback")]
    public Transform visualRoot; // objeto visual que pode 'afundar' ao pressionar
    public float pressDepth = 0.08f;
    public float animationTime = 0.08f;
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    // estado interno
    private HashSet<GameObject> activators = new HashSet<GameObject>();
    private bool isPressed = false;
    private Vector3 visualClosedLocalPos;

    private void Awake()
    {
        if (visualRoot == null) visualRoot = transform;
        visualClosedLocalPos = visualRoot.localPosition;
    }

    private void Start()
    {
        // Garantir que o collider é trigger (recomendado)
        var col = GetComponent<Collider2D>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"[ButtonController2D] Collider on {name} is not set to Trigger. Recommended to use a Trigger collider for detection.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject root = GetRootObjectFromCollider(collision);
        if (!IsActivator(root)) return;

        if (activators.Add(root))
        {
            UpdatePressedState();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject root = GetRootObjectFromCollider(collision);
        if (!IsActivator(root)) return;

        if (activators.Remove(root))
        {
            UpdatePressedState();
        }
    }

    private GameObject GetRootObjectFromCollider(Collider2D col)
    {
        if (col.attachedRigidbody != null)
            return col.attachedRigidbody.gameObject;
        return col.gameObject;
    }

    private bool IsActivator(GameObject go)
    {
        return (activatorLayers.value & (1 << go.layer)) != 0;
    }

    private void UpdatePressedState()
    {
        bool shouldBePressed = activators.Count > 0;

        if (shouldBePressed == isPressed) return;

        isPressed = shouldBePressed;

        // Reporta ao sistema central
        ButtonSystem.ReportButtonState(color, isPressed);

        if (isPressed)
        {
            onPressed?.Invoke();
            StopAllCoroutines();
            StartCoroutine(AnimateVisual(visualClosedLocalPos, visualClosedLocalPos + Vector3.down * pressDepth, animationTime));
        }
        else
        {
            onReleased?.Invoke();
            StopAllCoroutines();
            StartCoroutine(AnimateVisual(visualRoot.localPosition, visualClosedLocalPos, animationTime));
        }
    }

    private IEnumerator AnimateVisual(Vector3 from, Vector3 to, float time)
    {
        float t = 0f;
        while (t < time)
        {
            visualRoot.localPosition = Vector3.Lerp(from, to, t / time);
            t += Time.deltaTime;
            yield return null;
        }
        visualRoot.localPosition = to;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Color giz = Color.white;
        switch (color)
        {
            case DoorColorEnum.Red: giz = Color.red; break;
            case DoorColorEnum.Blue: giz = Color.blue; break;
            case DoorColorEnum.Green: giz = Color.green; break;
            case DoorColorEnum.Yellow: giz = Color.yellow; break;
        }
        Gizmos.color = giz;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.1f, Vector3.one * 0.3f);
    }
#endif
}
