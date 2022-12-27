using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator 
{
    public ColorSettings ColorSettings;
    public INoiseFilter biomeNoiseFilter;

    public Texture2D Texture;

    const int Resolution=50;


    public void UpdateSettings(ColorSettings settings)
    {
        this.ColorSettings=settings;

        if(Texture==null||Texture.height!=settings.biomeColorSettings.biomes.Length){
        Texture=new Texture2D(Resolution,settings.biomeColorSettings.biomes.Length);
        }
        biomeNoiseFilter=NoiseFilterFactory.CreateNoiseFilter(ColorSettings.biomeColorSettings.NoiseSettings);
    }

    public void SetMinMax(MinMax ElMinMax){
        ColorSettings.ShaderMaterial.SetVector("_ElevationMinMax",new Vector4(ElMinMax.Min,ElMinMax.Max));
    }
    
    public float BiomeHeightPercent(Vector3 PointOnUnitSphere)//Classify unit vectors on spheres as to the biomes they belong to
    {
        float Height=(PointOnUnitSphere.y+1)/2f; //ranges from 0 to 1 according to height
        Height+=(biomeNoiseFilter.Evaluate(PointOnUnitSphere)-ColorSettings.biomeColorSettings.noiseOffset)*ColorSettings.biomeColorSettings.noiseStrength;
        float biomeIndex=0;
        int numBiomes=ColorSettings.biomeColorSettings.biomes.Length;
        float blendRange=ColorSettings.biomeColorSettings.BlendAmount/2f +0.001f;


        for (int i = 0; i < numBiomes; i++)
        {
            float BiomeStartHeight=ColorSettings.biomeColorSettings.biomes[i].StartHeight;
            float dst=Height-BiomeStartHeight;
            float weight=Mathf.InverseLerp(-blendRange,blendRange,dst);
            biomeIndex*=(1-weight);
            biomeIndex+=i*weight;
        }
        return biomeIndex/Mathf.Max(1,(numBiomes-1));
    }


    public void UpdateTexColors(){
        Color[] Colors= new Color[Texture.width*Texture.height];
        int ColorIndex=0;

    foreach (var Biome in ColorSettings.biomeColorSettings.biomes){
        for (int i = 0; i < Resolution; i++)
        {
            Color GradientColor=Biome.Gradient.Evaluate(i/(Resolution-1f));
            Color Tintcolor=Biome.Tint;
            Colors[ColorIndex]=GradientColor*(1-Biome.TintPercent)+Tintcolor*Biome.TintPercent;
            ColorIndex++;
        }
    }
        Texture.SetPixels(Colors);
        Texture.Apply();
        ColorSettings.ShaderMaterial.SetTexture("_ColorGradient",Texture);

    }
}
