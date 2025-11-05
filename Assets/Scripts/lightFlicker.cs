using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchLightFlicker : MonoBehaviour
{
    [Header("Referência da Luz")]
    [SerializeField] private Light2D torchLight;

    [Header("Intensidade do Flicker")]
    [SerializeField] private float minIntensity = 3f;
    [SerializeField] private float maxIntensity = 5f;

    [Header("Velocidade da Variação")]
    [SerializeField] private float flickerSpeed = 10f;

    [Header("Variação Aleatória")]
    [SerializeField] private float randomRange = 0.2f;

    [Header("Cores do Fogo")]
    [SerializeField] private Color colorA = new Color(1f, 0.8f, 0.3f); // Amarelo quente
    [SerializeField] private Color colorB = new Color(1f, 0.5f, 0.1f); // Laranja avermelhado
    [SerializeField] private float colorChangeSpeed = 3f;

    private float targetIntensity;
    private Color targetColor;

    void Start()
    {
        if (torchLight == null)
            torchLight = GetComponent<Light2D>();

        targetIntensity = torchLight.intensity;
        targetColor = colorA;
    }

    void Update()
    {
        // Alvo de intensidade oscila naturalmente
        float newIntensity = Mathf.Lerp(targetIntensity,
            Random.Range(minIntensity, maxIntensity),
            Time.deltaTime * flickerSpeed);

        // Adiciona uma leve variação randômica (ruído)
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f) * randomRange;

        torchLight.intensity = newIntensity + noise;
        targetIntensity = newIntensity;

        // Alterna suavemente entre as duas cores
        float t = (Mathf.Sin(Time.time * colorChangeSpeed) + 1f) / 2f;
        Color newColor = Color.Lerp(colorA, colorB, t);

        // Pequena variação randômica para evitar repetição idêntica em várias tochas
        newColor.r += Random.Range(-0.02f, 0.02f);
        newColor.g += Random.Range(-0.02f, 0.02f);
        newColor.b += Random.Range(-0.02f, 0.02f);

        torchLight.color = newColor;
    }
}
