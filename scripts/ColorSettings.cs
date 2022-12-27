using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ColorSettings : ScriptableObject
{

    public Material ShaderMaterial;
    public BiomeColorSettings biomeColorSettings;

    [System.Serializable]
    public class BiomeColorSettings{
        
        public NoiseSettings NoiseSettings;
        public float noiseOffset;
        public float noiseStrength;
        [Range(0,1)]
        public float BlendAmount;

        public Biome[] biomes;
        [System.Serializable]
        public class Biome{

            public Color Tint;
            [Range(0,1)]
            public float TintPercent;
            [Range(0,1)]
            public float StartHeight;
            public Gradient Gradient;

        }
    }
}
