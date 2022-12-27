using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    public ShapeSettings shapeSettings;
    INoiseFilter[] filters;
    public MinMax ElevationMinMax;
    

  
    public void UpdateSettings(ShapeSettings settings)
    {
        shapeSettings = settings;
        filters = new  INoiseFilter[shapeSettings.noiseLayers.Length];
        for (int i = 0; i < filters.Length; i++)
        {
            filters[i] = NoiseFilterFactory.CreateNoiseFilter(shapeSettings.noiseLayers[i].Settings);
        }
        ElevationMinMax=new MinMax();
    }
    public Vector3 CalculatePointonPlanet(Vector3 unitPointvec)
    {
        float elevation = 0;
        float FirstLayerVal =1;


        if (filters.Length > 0)
        {
            FirstLayerVal = filters[0].Evaluate(unitPointvec);
            if (shapeSettings.noiseLayers[0].enabled)
            {
                elevation = FirstLayerVal;
            }
        }

        for (int i = 1; i < filters.Length; i++)
        {
            if (shapeSettings.noiseLayers[i].enabled)
            {
                float mask = (shapeSettings.noiseLayers[i].useFirstLayerAsMask) ? FirstLayerVal : 1;
                elevation += filters[i].Evaluate(unitPointvec)*mask;
            }
        }

        elevation*=AudioVisualEnabled();
        elevation=shapeSettings.Radius*(elevation+1);
        ElevationMinMax.AddValue(elevation);

       

        return unitPointvec * elevation;//elevtion is from 0 yo 1 Convert it to 1 to 0 =>increase size
    }
    public float AudioVisualEnabled()
    {
        if(shapeSettings.audio==null)
            return 1;
        
        return shapeSettings.audio.VisualiseHeight();

    }
}
