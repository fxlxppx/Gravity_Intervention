using UnityEngine;

public class WeaponTrapController : MonoBehaviour
{
    [Header("Configuração da Arma")]
    public DoorColorEnum color = DoorColorEnum.Red; // mesma lógica dos portões/botões
    public GameObject rockPrefab;                   // prefab da pedra redonda
    public Transform spawnPoint;                    // onde a pedra nasce
    public float fireCooldown = 2f;                 // tempo entre disparos

    private float cooldownTimer = 0f;

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public void Activate()
    {
        if (cooldownTimer <= 0f)
        {
            ShootRock();
            cooldownTimer = fireCooldown;
        }
    }

    private void ShootRock()
    {
        if (rockPrefab == null) return;

        Transform point = spawnPoint != null ? spawnPoint : transform;

        GameObject rock = Instantiate(rockPrefab, point.position, Quaternion.identity);

        Debug.Log("Arma disparou pedra!");
    }
}
