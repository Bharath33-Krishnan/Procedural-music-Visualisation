using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseFilter:INoiseFilter 
{
    Noise noise = new Noise();
    NoiseSettings.SimpleNoiseSettings Settings;

    public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings NoiseSettings)
    {
        this.Settings = NoiseSettings;
    }
    public float Evaluate(Vector3 point)
    {
        float Noiseval=0;
        float frequency = Settings.baseRougness;
        float amplitude = 1;

        for (int i = 0; i < Settings.NumberOfLayers; i++)
        {
            float v = noise.Evaluate(point * frequency + Settings.centre);
            Noiseval += (v + 1) * .5f * amplitude;//to make v between 0 and 1
            frequency *= Settings.roughness;
            amplitude *= Settings.persistance;
        }
        Noiseval = Mathf.Max(0, Noiseval - Settings.MinValue);
        return Noiseval*Settings.strength;
    }
    
}
