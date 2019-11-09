using NVIDIA.Flex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckParticleCollision : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);
    }

    private void Start()
    {
        StartCoroutine(DestroyParticles());
    }

    IEnumerator DestroyParticles()
    {
        yield return new WaitForSeconds(5f);

        ParticleSystem ps = GetComponent<ParticleSystem>();
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[0];

        Debug.Log(ps.GetParticles(particles));

        FlexActor flexActor = GetComponent<FlexActor>();
        flexActor.enabled = false;

        FlexParticleController pc = GetComponent<FlexParticleController>();
        pc.enabled = false;

        ps.Clear();

        Debug.Log(ps.GetParticles(particles));

        yield return null;
    }
}
