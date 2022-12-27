using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShapeSettings : ScriptableObject
{
    public float Radius;
    public NoiseLayer[] noiseLayers;
    public AUdio audio;

    [System.Serializable()]
    public class NoiseLayer
    {
        public bool useFirstLayerAsMask;
        public bool enabled = true;
        public NoiseSettings Settings;
        
        

       

    }

}
