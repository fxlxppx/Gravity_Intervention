using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(ParticleAttractor))]
public class GravityParticlesController : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleAttractor attractor;
    private ParticleSystem.EmissionModule emission;
    private ParticleSystem.MainModule main;

    [Header("Efeitos de Transição")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.8f;
    [SerializeField] private float maxEmissionRate = 100f;

    private Coroutine transitionRoutine;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        attractor = GetComponent<ParticleAttractor>();
        emission = ps.emission;
        main = ps.main;

        emission.rateOverTime = 0f;
        ps.Stop();
        attractor.enabled = false;
    }

    void OnEnable()
    {
        PlayerControl.OnGravityInverted += OnGravityInverted;
        PlayerControl.OnGravityReset += OnGravityReset;
    }

    void OnDisable()
    {
        PlayerControl.OnGravityInverted -= OnGravityInverted;
        PlayerControl.OnGravityReset -= OnGravityReset;
    }

    private void OnGravityInverted(float duration)
    {
        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        ps.Play();
        attractor.enabled = true;
        transitionRoutine = StartCoroutine(FadeIn(duration));
    }

    private void OnGravityReset()
    {
        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn(float duration)
    {
        float timer = 0f;
        emission.rateOverTime = 0f;

        // Fade-in da emissão
        while (timer < fadeInDuration)
        {
            float t = timer / fadeInDuration;
            emission.rateOverTime = Mathf.Lerp(0f, maxEmissionRate, t);
            timer += Time.deltaTime;
            yield return null;
        }

        emission.rateOverTime = maxEmissionRate;

        // Espera o tempo da gravidade invertida
        yield return new WaitForSeconds(duration - fadeOutDuration);

        // Inicia o fade-out automático
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float startRate = emission.rateOverTime.constant;
        float timer = 0f;

        // Fade-out da emissão
        while (timer < fadeOutDuration)
        {
            float t = timer / fadeOutDuration;
            emission.rateOverTime = Mathf.Lerp(startRate, 0f, t);
            timer += Time.deltaTime;
            yield return null;
        }

        emission.rateOverTime = 0f;
        attractor.enabled = false;
        ps.Stop();
    }
}
