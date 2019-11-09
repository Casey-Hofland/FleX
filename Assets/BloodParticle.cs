using NVIDIA.Flex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BloodParticle : MonoBehaviour
{
    [SerializeField] private GameObject sprite;
    [SerializeField] private float dryTime;

    private new ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;

    private int cp = 0;

    private void Start()
    {
        InitializeParticleSystem();

        InvokeRepeating("DestroyOneByOne", 2f, 1f / 12f);
        //Invoke("DestroyStuff", 5f);
    }

    private void DestroyStuff()
    {
        Destroy(this);
        Destroy(gameObject);
    }

    private void DestroyOneByOne()
    {
        int alive = particleSystem.GetParticles(particles);

        var main = particleSystem.main;
        main.maxParticles = 0;

        FlexActor flexActor = GetComponent<FlexActor>();

        if (main.maxParticles <= 0)
        {
            CancelInvoke();
        }

        particleSystem.SetParticles(particles, alive);
    }

    private void InitializeParticleSystem()
    {
        if (particleSystem == null)
            particleSystem = GetComponent<ParticleSystem>();

        if (particles == null || particles.Length < particleSystem.main.maxParticles)
            particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
    }

    private void LateUpdate()
    {
        int alive = particleSystem.GetParticles(particles);

        var particle = particles[0];
        //Debug.LogFormat("Animated: {0} - Velocity: {1} - TotalVelocity: {2}", particle.animatedVelocity, particle.velocity, particle.totalVelocity);

        particleSystem.SetParticles(particles, alive);
    }

    private void OnDestroy()
    {
        Debug.Log("Called");

        int alive = particleSystem.GetParticles(particles);

        for (int i = 0; i < alive; i++)
        {
            var particle = particles[i];

            if (i == 0)
            {
                Debug.LogFormat("Animated: {0} - Velocity: {1} - TotalVelocity: {2}", particle.animatedVelocity, particle.velocity, particle.totalVelocity);
            }

            Instantiate(sprite, particles[i].position, Quaternion.identity);
        }

        particleSystem.SetParticles(particles, alive);
    }
}
