using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WeaponTrapController : MonoBehaviour
{
    [Header("Configuração da Arma")]
    public DoorColorEnum color = DoorColorEnum.Red; // mesma lógica dos portões/botões
    public GameObject rockPrefab;                   // prefab da pedra redonda
    public Transform spawnPoint;                    // onde a pedra nasce
    public float fireCooldown = 2f;                 // tempo entre disparos

    [Header("Indicador Visual")]
    [Tooltip("Luz que indica se a arma está pronta para disparar")]
    [SerializeField] private Light2D readyLight;

    private float cooldownTimer = 0f;

    private void Start()
    {
        UpdateLightState(true);
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                cooldownTimer = 0f;
                UpdateLightState(true);
            }
        }
    }

    public void Activate()
    {
        if (cooldownTimer <= 0f)
        {
            ShootRock();
            cooldownTimer = fireCooldown;

            UpdateLightState(false);
        }
    }

    private void ShootRock()
    {
        if (rockPrefab == null) return;

        Transform point = spawnPoint != null ? spawnPoint : transform;

        GameObject rock = Instantiate(rockPrefab, point.position, Quaternion.identity);

        Debug.Log("Arma disparou pedra!");
    }

    private void UpdateLightState(bool isReady)
    {
        if (readyLight == null) return;

        readyLight.enabled = isReady;

        readyLight.intensity = isReady ? 1f : 0f;
    }
}
