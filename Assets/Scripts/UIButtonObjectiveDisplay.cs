using UnityEngine;
using UnityEngine.UI;

public class UIButtonObjectiveDisplay : MonoBehaviour
{
    public static UIButtonObjectiveDisplay Instance { get; private set; }

    [Header("Sprites por cor")]
    public Sprite redSprite;
    public Sprite blueSprite;
    public Sprite greenSprite;
    public Sprite yellowSprite;

    [Header("Slots de ícones (máximo 3)")]
    [Tooltip("Ícones na UI, em ordem")]
    public Image[] buttonIcons;

    private int requiredButtons = 0;
    private int activatedButtons = 0;
    private Sprite currentColorSprite;

    private DoorColorEnum trackedColor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void OnEnable()
    {
        ButtonSystem.OnButtonPressedCountChanged += HandleButtonUpdate;
        CheckpointManager.OnPlayerRespawn += ResetObjective;
    }

    private void OnDisable()
    {
        ButtonSystem.OnButtonPressedCountChanged -= HandleButtonUpdate;
        CheckpointManager.OnPlayerRespawn -= ResetObjective;
    }

    private void Start()
    {
        HideObjective();
    }

    public void InitializeObjective(DoorColorEnum color, int required)
    {
        trackedColor = color;
        requiredButtons = Mathf.Clamp(required, 1, 3);
        activatedButtons = 0;

        currentColorSprite = GetSpriteForColor(color);

        SetupIcons();
        ShowObjective();
    }

    private void HandleButtonUpdate(DoorColorEnum color, int count)
    {
        if (color != trackedColor)
            return;

        activatedButtons = Mathf.Clamp(count, 0, requiredButtons);

        if (activatedButtons >= requiredButtons)
        {
            activatedButtons = requiredButtons;
        }

        UpdateIcons();
    }

    private Sprite GetSpriteForColor(DoorColorEnum color)
    {
        switch (color)
        {
            case DoorColorEnum.Red: return redSprite;
            case DoorColorEnum.Blue: return blueSprite;
            case DoorColorEnum.Green: return greenSprite;
            case DoorColorEnum.Yellow: return yellowSprite;
        }

        return null;
    }

    private void SetupIcons()
    {
        for (int i = 0; i < buttonIcons.Length; i++)
        {
            if (i < requiredButtons)
            {
                buttonIcons[i].gameObject.SetActive(true);
                buttonIcons[i].sprite = currentColorSprite;
                buttonIcons[i].color = new Color(1, 1, 1, 0.3f);
            }
            else
            {
                buttonIcons[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateIcons()
    {
        for (int i = 0; i < buttonIcons.Length; i++)
        {
            if (!buttonIcons[i].gameObject.activeSelf)
                continue;

            buttonIcons[i].color = (i < activatedButtons)
                ? Color.white
                : new Color(1, 1, 1, 0.3f);
        }
    }

    private void ResetObjective()
    {
        activatedButtons = 0;
        UpdateIcons();
    }
    public void SetFilledCount(int count)
    {
        activatedButtons = Mathf.Clamp(count, 0, requiredButtons);
        UpdateIcons();

    }
    public void ShowObjective()
    {
        gameObject.SetActive(true);
        UpdateIcons();
    }

    public void HideObjective()
    {
        gameObject.SetActive(false);
    }
}
