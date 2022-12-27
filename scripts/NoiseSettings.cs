using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings 
{
    public enum noiseType{simple,rigid};
    public noiseType NoiseType;
    [ConditionalHide("NoiseType",0)]
    public SimpleNoiseSettings simpleNoiseSettings;
     [ConditionalHide("NoiseType",1)]
    public RigidNoiseSettings rigidNoiseSettings;


    [System.Serializable]
    public class SimpleNoiseSettings{
        public float strength = 1;
        public float baseRougness=1;
        [Range(1,10)]
        public int NumberOfLayers = 1;
        public float persistance = .5f;
        public float roughness = 2;
        public Vector3 centre;

        public float MinValue;//allows to recede into base sphere
    }

    [System.Serializable]
    public class RigidNoiseSettings:SimpleNoiseSettings{
        public float WeightMultiplier=.8f;
    }

    



}
