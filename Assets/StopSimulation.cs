using NVIDIA.Flex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSimulation : MonoBehaviour
{
    [SerializeField] private FlexContainer driedContainer;

    void Start()
    {
        Invoke("S", 5f);
    }

    private void S()
    {
        Debug.Log("S");

        //Destroy(GetComponent<FlexParticleController>());
        //Destroy(GetComponent<FlexFluidRenderer>());

        FlexActor flexActor = GetComponent<FlexActor>();
        flexActor.container = driedContainer;

        Destroy(this);

        /*
        int[] particles = flexActor.container.AllocParticles(flexActor.container.maxParticles);

        Debug.LogFormat("Max: {0} - Found: {1}", flexActor.container.maxParticles, particles.Length);

        flexActor.container.FreeParticles(particles);
        flexActor.asset.ClearFixedParticles();

        for (int i = 0; i < particles.Length; i++)
        {

        }
        */
        
        //NVIDIA.Flex.FlexExt.FreeParticles((FlexExt.Container)flexActor.container, flexActor.container.maxParticles, flexActor.asset.maxParticles);

        //Destroy(GetComponent<FlexActor>());

        //GetComponent<ParticleSystemRenderer>().enabled = true;
    }
}
