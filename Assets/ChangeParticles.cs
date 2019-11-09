using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ChangeParticles : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        InvokeRepeating("ChangeDirection", 2f, 2f);
    }

    void ChangeDirection()
    {
        if (particles == null || particles.Length < particleSystem.main.maxParticles)
            particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];

        int alive = particleSystem.GetParticles(particles);

        for (int i = 0; i < alive; i++)
        {
            Vector3 newPos = particles[i].position;
            newPos.z -= 10f;
            particles[i].position = newPos;
        }

        particleSystem.SetParticles(particles, alive);
    }
}
