using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilterFactory : MonoBehaviour
{
    public static INoiseFilter CreateNoiseFilter(NoiseSettings settings){
        switch(settings.NoiseType){
            case NoiseSettings.noiseType.simple:
            {
                return new SimpleNoiseFilter(settings.simpleNoiseSettings);
            }
            case NoiseSettings.noiseType.rigid:
            {
                return new RigidNoiseFilter(settings.rigidNoiseSettings);
            }
            
        }
        return null;

    }
}
