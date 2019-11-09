using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;

public class FlexFoamFluidRenderer : FlexFluidRenderer
{
    [SerializeField] private float foamSpeed;
    private float sqrFoamSpeed;
    [SerializeField] private GameObject foam;

    private void Start()
    {
        sqrFoamSpeed = foamSpeed * foamSpeed;
    }

    protected override void OnFlexUpdate(FlexContainer.ParticleData _particleData)
    {
        base.OnFlexUpdate(_particleData);

        /*
        if (Application.isPlaying)
        {
            Flex.Buffer buffer = Flex.AllocBuffer(FlexContainer.library, m_actor.container.maxParticles, sizeof(float) * 4, Flex.BufferType.Host);
            Flex.SetDiffuseParticles(m_actor.container.solver, buffer, buffer, m_actor.indexCount);

            foreach (var index in m_actor.indices)
            {
                float sqrSpeed = _particleData.GetVelocity(index).sqrMagnitude;
                if (sqrSpeed >= sqrFoamSpeed)
                {
                    Vector3 pos = _particleData.GetParticle(index);
                    Instantiate(foam, pos, Quaternion.identity, transform);
                }
            }
        }
        */
    }
}
