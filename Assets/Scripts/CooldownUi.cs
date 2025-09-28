using UnityEngine;

public class CooldownUI : MonoBehaviour
{
    public static CooldownUI Instance;

    [SerializeField] private Animator animator;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void StartCooldown(float time)
    {
        gameObject.SetActive(true);

        if (animator != null)
        {
            animator.SetTrigger("Play");
        }
    }

    public void FinishCooldown()
    {
        gameObject.SetActive(false);
    }

}
