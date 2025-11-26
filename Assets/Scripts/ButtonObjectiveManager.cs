using UnityEngine;

public class ButtonObjectiveManager : MonoBehaviour
{
    public static ButtonObjectiveManager Instance { get; private set; }

    private int requiredButtons = 0;
    private int activatedButtons = 0;
    private DoorColorEnum currentColor;

    [Header("Configuração")]
    [Tooltip("Tempo que os botões precisam permanecer ativados antes de esconder o objetivo")]
    public float completionDelay = 0.5f;

    private Coroutine hideRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void OnEnable()
    {
        ButtonSystem.OnButtonPressedCountChanged += HandleButtonCount;
        CheckpointManager.OnPlayerRespawn += ResetState;
    }

    private void OnDisable()
    {
        ButtonSystem.OnButtonPressedCountChanged -= HandleButtonCount;
        CheckpointManager.OnPlayerRespawn -= ResetState;
    }

    public void InitializeRoomObjective(DoorColorEnum color, int buttonsNeeded)
    {
        currentColor = color;
        requiredButtons = Mathf.Clamp(buttonsNeeded, 1, 3);
        activatedButtons = 0;

        UIButtonObjectiveDisplay.Instance.InitializeObjective(color, requiredButtons);
    }

    private void HandleButtonCount(DoorColorEnum color, int pressedCount)
    {
        if (color != currentColor)
            return;

        activatedButtons = Mathf.Clamp(pressedCount, 0, requiredButtons);

        UIButtonObjectiveDisplay.Instance.SetFilledCount(activatedButtons);

        if (activatedButtons >= requiredButtons)
        {
            if (hideRoutine != null)
                StopCoroutine(hideRoutine);

            hideRoutine = StartCoroutine(HideAfterDelay());
        }
        else
        {
            if (hideRoutine != null)
            {
                StopCoroutine(hideRoutine);
                hideRoutine = null;
            }
        }
    }

    private System.Collections.IEnumerator HideAfterDelay()
    {
        float t = 0f;

        while (t < completionDelay)
        {
            if (activatedButtons < requiredButtons)
                yield break;

            t += Time.deltaTime;
            yield return null;
        }

        UIButtonObjectiveDisplay.Instance.HideObjective();
        hideRoutine = null;
    }

    private void ResetState()
    {
        activatedButtons = 0;

        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }
    }
}
