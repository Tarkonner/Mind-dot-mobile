using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipulParticalController : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particleSystems = new();

    
    public void PlayParticles()
    {
        foreach (ParticleSystem item in particleSystems)
        {
            item.Play();
        }
    }
}
