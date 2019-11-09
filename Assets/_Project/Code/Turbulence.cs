// The methods corresponding to flagParents in this script is not well thought-out, take care with
// the methods "RetrieveFlagParents()" and "UpdateParents()". All Wind code can be regarded as relatively save.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NVIDIA.Flex;
using System.Linq;
using Extensions;

using Random = UnityEngine.Random;

public class Turbulence : MonoBehaviour
{
    [SerializeField] private FlexContainer container;

    [SerializeField] private Vector3 windDirection = Vector3.back;
    [SerializeField] private Wind wind = new Wind();
    [SerializeField] private Wind burstWind = new Wind(new Vector2(3f, 6f), .4f, new Vector2(1f, 4f), .25f);

    [SerializeField] private StartingWind startingWind = StartingWind.Wind;
    
    private enum StartingWind
    {
        Wind,
        Burst
    };

    [Serializable]
    private class Wind
    {
        public Vector2 speed = new Vector2(.25f, .5f);
        [Range(0f, 1f)] public float swivel = .1f;
        public Vector2 duration = new Vector2(.05f, 25f);
        public float changeDuration = 1f;

        private float currentDuration = 0f;

        private LerpValue lerpSpeed;
        private LerpValue lerpSwivel;

        public bool HasEnded { get { return (currentDuration <= 0f); } }
        public float currentSpeed {  get { return lerpSpeed.current; } }
        public float currentSwivel { get { return lerpSwivel.current; } }

        public Wind()
        {
            lerpSpeed = new LerpValue(changeDuration, () => speed.RandomValue());
            lerpSwivel = new LerpValue(changeDuration, () => Random.Range(-swivel, swivel) * 180, true);
        }

        public Wind(Vector2 speed, float swivel, Vector2 duration, float changeDuration) : this()
        {
            this.speed = speed;
            this.swivel = Mathf.Clamp01(swivel);
            this.duration = duration;
            this.changeDuration = changeDuration;
        }

        public void Init(float currentSpeed, float currentSwivel)
        {
            currentDuration = duration.RandomValue();

            lerpSpeed.Init(currentSpeed);
            lerpSwivel.Init(currentSwivel);
        }

        public void UpdateWind()
        {
            lerpSpeed.Update();
            lerpSwivel.Update();

            currentDuration -= Time.deltaTime;
        }

        private class LerpValue
        {
            public float current = 0f;
            private float last = 0f;
            private float target = 0f;
            private float t = 0f;
            private float changeTime = 0f;
            private bool cosineInterpolate = false;

            private Func<float> func;

            public LerpValue(float changeTime, Func<float> func, bool cosineInterpolate = false)
            {
                last = current;
                this.changeTime = changeTime;
                this.func = func;
                this.cosineInterpolate = cosineInterpolate;
            }

            public void Init(float current)
            {
                last = current;
                target = func();
                t = 0f;
            }

            public void Update()
            {
                float t2 = t;
                if (cosineInterpolate)
                {
                    t2 = (1 - Mathf.Cos(t * Mathf.PI)) / 2;
                }

                current = Mathf.Lerp(last, target, t2);
                t = Mathf.Clamp01(t + Time.deltaTime / changeTime);

                if (t >= 1 - float.Epsilon)
                {
                    last = current;
                    target = func();
                    t = 0;
                }
            }
        }
    }

    private Wind currentWind;
    private Vector3 currentWindDirection;
    private Transform[] flagParents;

    private void OnValidate()
    {
        wind.changeDuration = Mathf.Max(0.01f, wind.changeDuration);
        burstWind.changeDuration = Mathf.Max(0.01f, burstWind.changeDuration);
    }

    private void Awake()
    {
        windDirection.Normalize();
    }

    private void Start()
    {
        InitializeWind();
        RetrieveFlagParents();

        container.onFlexUpdate += OnFlexUpdate;
    }

    private void RetrieveFlagParents()
    {
        flagParents =
                    (from actor in FindObjectsOfType<FlexClothActor>()
                     where actor.CompareTag("Flag")
                     select actor.transform.parent)
                     .ToArray();
    }

    private void InitializeWind()
    {
        // Set the current wind type
        switch (startingWind)
        {
            case StartingWind.Burst:
                currentWind = burstWind;
                break;
            default:
                currentWind = wind;
                break;
        }

        // Initialize the current wind type with the current speed and angle of the wind
        // Angle is relative to the desired angle.
        Flex.Params windParams = new Flex.Params();
        Flex.GetParams(container.solver, ref windParams);

        float currentSpeed = windParams.wind.magnitude;
        float currentSwivel = Vector3.Angle(windParams.wind.normalized, windDirection);
        currentWind.Init(currentSpeed, currentSwivel);
    }

    private void OnFlexUpdate(FlexContainer.ParticleData particleData)
    {
        if (!Application.isPlaying)
            return;

        if (container && container.solver)
        {
            UpdateWind();
        }
    }

    private void UpdateWind()
    {
        // Retrieve the wind from the Flex Solver
        Flex.Params windParams = new Flex.Params();
        Flex.GetParams(container.solver, ref windParams);

        // Swap the windtype if the current windtype has exceeded its duration
        if (currentWind.HasEnded)
        {
            float lastWindSpeed = currentWind.currentSpeed;
            float lastSwivel = currentWind.currentSwivel;
            currentWind = (currentWind.Equals(wind)) ? burstWind : wind;
            currentWind.Init(lastWindSpeed, lastSwivel);
        }

        // Update and set the wind values and rotate the flag parents to look in the right direction
        currentWind.UpdateWind();
        currentWindDirection = Quaternion.AngleAxis(currentWind.currentSwivel, Vector3.up) * windDirection;
        currentWindDirection.Normalize();
        windParams.wind = currentWind.currentSpeed * currentWindDirection;
        windParams.wind.y = currentWind.currentSpeed / 8f; // <-- MAGIC NUMBER !!! TODO: create a function for upward wind lift.
        RotateParents();

        // Update the wind in the Flex Solver
        Flex.SetParams(container.solver, ref windParams);
    }

    private void RotateParents()
    {
        Vector3 lookDirection = currentWindDirection * -1;

        foreach (var parent in flagParents)
        {
            parent.localRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }
    }
}
