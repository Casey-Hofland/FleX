using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;

[RequireComponent(typeof(FlexParticleController))]
public class FoamParticle : MonoBehaviour
{
    private ParticleSystemRenderer particleSystemRenderer;
    private float startTime = 0f;
    private float remainingTime = 0f;

    private void Start()
    {
        particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
        remainingTime = startTime = GetComponent<ParticleSystem>().main.startLifetime.constant;
    }

    private void Update()
    {
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
            Destroy(gameObject);

        FadeFoam();
    }

    private void FadeFoam()
    {
        Color newColor = particleSystemRenderer.material.color;
        newColor.a = remainingTime / startTime;
        particleSystemRenderer.material.color = newColor;
    }
}
