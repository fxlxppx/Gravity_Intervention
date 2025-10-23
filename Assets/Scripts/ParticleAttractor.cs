using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAttractor : MonoBehaviour
{
    public Transform player;
    public float force = 10f;

    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void LateUpdate()
    {
        if (player == null) return;

        if (particles == null || particles.Length < ps.main.maxParticles)
            particles = new ParticleSystem.Particle[ps.main.maxParticles];

        int count = ps.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            Vector3 direction = (player.position - particles[i].position).normalized;
            particles[i].velocity += direction * force * Time.deltaTime;
        }

        ps.SetParticles(particles, count);
    }
}
