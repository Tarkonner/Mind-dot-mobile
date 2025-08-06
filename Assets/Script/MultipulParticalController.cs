using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipulParticalController : MonoBehaviour
{
    public static MultipulParticalController Instance;

    [SerializeField] List<ParticleSystem> particleSystems = new();

    private void Awake()
    {
        Instance = this;
    }

    public void PlayParticles()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps == null) continue;

            // Reset the particle system to allow re-triggering
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();
        }
    }
}
