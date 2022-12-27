using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidNoiseFilter:INoiseFilter 
{
    Noise noise = new Noise();
    NoiseSettings.RigidNoiseSettings Settings;

    public RigidNoiseFilter(NoiseSettings.RigidNoiseSettings NoiseSettings)
    {
        this.Settings = NoiseSettings;
    }
    public float Evaluate(Vector3 point)
    {
        float Noiseval=0;
        float frequency = Settings.baseRougness;
        float amplitude = 1;
        float weight=1;

        for (int i = 0; i < Settings.NumberOfLayers; i++)
        {
            float v = 1 - Mathf.Abs(noise.Evaluate(point * frequency + Settings.centre));
            v*=v;
            v*=weight;
            weight=Mathf.Clamp01(v*Settings.WeightMultiplier);
            Noiseval += (v) * amplitude;
            frequency *= Settings.roughness;
            amplitude *= Settings.persistance;
        }
        Noiseval = Mathf.Max(0, Noiseval - Settings.MinValue);
        return Noiseval*Settings.strength;
    }
}
