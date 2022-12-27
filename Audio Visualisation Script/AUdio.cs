using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AUdio : MonoBehaviour
{
    public Slider Audioslider;
    AudioSource source;
    Planet planet;
    public float[] spectrumdata = new float[512];

    public float[] FrequencyBands = new float[8];

    public static int FilterId=0;  
     
    public BeatSequencing Sequencing=BeatSequencing.Key;
    
    BeatSequencing CurrentSequence=BeatSequencing.Base;

    ShapeSettings layer;
    float BeatSequencer=0;

   
  
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        planet = GetComponent<Planet>();
        layer = planet.shapeSettings;
        layer.noiseLayers[0].Settings.simpleNoiseSettings.centre=Vector3.zero;
       
    }


    // Update is called once per frame
    void Update()
    { 
        SplitBands();
        MakeFrequencyBands(); 
        Visualise(BeatSequencer);
        UpdateAudio();
      
        
    }

    void SplitBands()
    {
         source.GetSpectrumData(spectrumdata, 0, FFTWindow.Blackman);


    }
    float Base;
    float pitch;
    float key;
    public void Visualise(float BeatSequencer){
       
        switch(Sequencing){
            
            case BeatSequencing.Base:
                BeatSequencer=Base;
                break;
            case BeatSequencing.Key:
                BeatSequencer=key;
                break;
            case BeatSequencing.Pitch:
                BeatSequencer=pitch;
                break;   
        }
        

        
         NoiseSettings settings= layer.noiseLayers[0].Settings;
         Base =  FrequencyBands[1] + FrequencyBands[2];
         pitch = FrequencyBands[3] + FrequencyBands[4];
         key=FrequencyBands[5]+ FrequencyBands[6]+ FrequencyBands[7];


        settings.simpleNoiseSettings.centre += new Vector3(Base, key, pitch)*BeatSequencer/5;
        //settings.strength = 0.5f + (pitch-0.01f);
        //settings.baseRougness = 1.7f - (Base - 0.01f) * 2;
        
        //settings.strength = Mathf.Max(0.5f, settings.strength - 0.005f);
        //planet.colorSettings.MeshColorGradient.colorKeys.SetValue(Color.red,0);   
        planet.OnColorSettingsUpdated();
        planet.OnShapeSettingsUpdated();    

    }
    public float VisualiseHeight()
    {
       
        
        

        switch(FilterId){

            default:
            {
                return 0;
                
            }
            case 2:
            {
                float B=(Base- 0.005f)*4;
                if(B<0.5f){
                    B=0.5f;
                }
                return Mathf.Min(B, 4);
            }
            case 0:
            {
                float p= (pitch - 0.005f)*4;
                if(p<0.5f){
                    p=0.5f;
                }
                return Mathf.Min(p,4);
            }
            case 4:
            {
                float k= (key - 0.005f)*8;
                if(k<0.5f){
                    k=0.5f;
                }
                return Mathf.Min(k, 4);
            }
            
        }
        

       
    }

    void UpdateAudio(){
        if(Audioslider==null || source==null || source.clip==null)
            return;
        
        float clipLength=source.clip.length-1;
        Audioslider.onValueChanged.AddListener(delegate{SkipAudio(clipLength);});
           
        Audioslider.value=source.time/clipLength;


    }
    void SkipAudio(float length){

        source.time=Audioslider.value*length;

    }
    void MakeFrequencyBands()
    {

        float average = 0;
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            int  samplecount = (int)Mathf.Pow(2, i + 1);
            if (i == 7) { samplecount += 2; }

            for (int j = 0; j < samplecount; j++)
            {
                average += spectrumdata[count]*(count+1);
                count++;
            }
            
            average /= count;
            FrequencyBands[i] = average;
        }

        
    }
}

public enum BeatSequencing{
    Key,
    Pitch,
    Base

};