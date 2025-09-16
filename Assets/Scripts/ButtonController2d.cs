using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class ButtonController2D : MonoBehaviour
{
    public enum ButtonMode
    {
        Hold,
        Toggle,
        Latch
    }

    [Header("Config")]
    public DoorColorEnum color = DoorColorEnum.Red;
    public LayerMask activatorLayers;

    [Tooltip("Modo de funcionamento do botão")]
    public ButtonMode buttonMode = ButtonMode.Hold;

    [Header("Visual / Feedback")]
    public Transform visualRoot;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject root = GetRootObjectFromCollider(collision);
        if (!IsActivator(root)) return;

        if (activators.Add(root))
        {
            HandlePress();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject root = GetRootObjectFromCollider(collision);
        if (!IsActivator(root)) return;

        if (activators.Remove(root))
        {
            HandleRelease();
        }
    }

    private void HandlePress()
    {
        switch (buttonMode)
        {
            case ButtonMode.Hold:
                UpdatePressedState(true);
                break;

            case ButtonMode.Toggle:
                UpdatePressedState(!isPressed); // alterna estado
                break;

            case ButtonMode.Latch:
                if (!isPressed) UpdatePressedState(true); // só ativa uma vez
                break;
        }
    }

    private void HandleRelease()
    {
        if (buttonMode == ButtonMode.Hold)
        {
            if (activators.Count == 0)
                UpdatePressedState(false);
        }
        // Toggle e Latch não fazem nada no Release
    }

    private void UpdatePressedState(bool state)
    {
        if (state == isPressed) return;

        isPressed = state;
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
