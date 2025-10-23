using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class GravityReadyExplosion : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Stop();
    }

    void OnEnable()
    {
        PlayerControl.OnGravityReady += TriggerExplosion;
    }

    void OnDisable()
    {
        PlayerControl.OnGravityReady -= TriggerExplosion;
    }

    private void TriggerExplosion()
    {
        ps.transform.position = PlayerControl.Instance.transform.position;
        ps.Play();
    }
}
