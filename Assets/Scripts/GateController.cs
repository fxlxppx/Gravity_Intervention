using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GateController2D : MonoBehaviour
{
    [Header("Config")]
    public DoorColorEnum color = DoorColorEnum.Red;
    [Tooltip("Quantos botões dessa cor precisam estar pressionados para abrir")]
    public int requiredButtonsToOpen = 1;

    [Header("Comportamento")]
    public GateBehavior behavior = GateBehavior.Default;
    public float autoCloseDelay = 3f;

    [Header("Animação")]
    public bool useAnimator = true;
    public Animator animator;
    public bool disableColliderWhenOpen = true;
    public Collider2D blockingCollider;

    [Header("Fallback Move Animation (se não usar Animator)")]
    public Vector3 openOffset = new Vector3(0, 2f, 0);
    public float moveTime = 0.25f;

    [Header("Partículas do Portão")]
    [SerializeField] private ParticleSystem gateParticles;

    private Vector3 closedLocalPos;
    private bool isOpen = false;
    private bool permanentlyOpen = false;
    private Coroutine autoCloseRoutine;

    private void Awake()
    {
        closedLocalPos = transform.localPosition;
        if (blockingCollider == null)
            blockingCollider = GetComponent<Collider2D>();

        if (gateParticles == null)
            gateParticles = GetComponentInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        ButtonSystem.OnButtonPressedCountChanged += OnColorCountChanged;
        CheckpointManager.OnPlayerRespawn += ResetToInitialState;
    }

    private void OnDisable()
    {
        ButtonSystem.OnButtonPressedCountChanged -= OnColorCountChanged;
        CheckpointManager.OnPlayerRespawn -= ResetToInitialState;
    }

    private void Start()
    {
        int initialCount = ButtonSystem.GetPressedCount(color);
        ApplyState(initialCount >= requiredButtonsToOpen);
    }

    private void OnColorCountChanged(DoorColorEnum changedColor, int pressedCount)
    {
        if (changedColor != color) return;
        if (behavior == GateBehavior.StayOpen && permanentlyOpen) return;

        bool shouldOpen = pressedCount >= requiredButtonsToOpen;

        if (behavior == GateBehavior.AutoCloseTimer)
        {
            if (shouldOpen)
            {
                if (autoCloseRoutine != null)
                {
                    StopCoroutine(autoCloseRoutine);
                    autoCloseRoutine = null;
                }
                ApplyState(true);
            }
            else if (isOpen && autoCloseRoutine == null)
            {
                autoCloseRoutine = StartCoroutine(AutoCloseAfterDelay(autoCloseDelay));
            }
        }
        else
        {
            ApplyState(shouldOpen);
        }
    }

    private void ApplyState(bool open)
    {
        if (open == isOpen) return;

        isOpen = open;
        if (isOpen && behavior == GateBehavior.StayOpen)
            permanentlyOpen = true;

        if (gateParticles != null)
        {
            if (isOpen && !gateParticles.isPlaying)
                gateParticles.Play();
            else if (!isOpen && gateParticles.isPlaying)
                gateParticles.Stop();
        }

        if (useAnimator && animator != null)
        {
            animator.SetBool("Open", isOpen);
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(MoveGate(isOpen ? closedLocalPos + openOffset : closedLocalPos, moveTime));
        }

        if (disableColliderWhenOpen && blockingCollider != null)
            blockingCollider.enabled = !isOpen;
    }

    private IEnumerator AutoCloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (behavior == GateBehavior.AutoCloseTimer && !permanentlyOpen)
        {
            autoCloseRoutine = null;
            ApplyState(false);
        }
    }

    private IEnumerator MoveGate(Vector3 targetLocalPos, float time)
    {
        if (gateParticles != null && !gateParticles.isPlaying)
            gateParticles.Play();

        Vector3 from = transform.localPosition;
        float t = 0f;
        while (t < time)
        {
            transform.localPosition = Vector3.Lerp(from, targetLocalPos, t / time);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetLocalPos;

        if (gateParticles != null && gateParticles.isPlaying)
            gateParticles.Stop();
    }

    private void ResetToInitialState()
    {
        if (autoCloseRoutine != null)
        {
            StopCoroutine(autoCloseRoutine);
            autoCloseRoutine = null;
        }

        isOpen = false;
        permanentlyOpen = false;

        if (useAnimator && animator != null)
        {
            animator.SetBool("Open", false);
        }
        else
        {
            transform.localPosition = closedLocalPos;
        }

        if (blockingCollider != null)
            blockingCollider.enabled = true;

        if (gateParticles != null)
            gateParticles.Stop();

        Debug.Log($"GateController2D ({color}): Resetado ao estado inicial.");
    }
}
