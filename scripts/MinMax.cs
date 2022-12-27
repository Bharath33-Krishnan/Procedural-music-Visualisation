using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax
{
    public float Min;
    public float Max;
    public MinMax(){
        Min=float.MaxValue;
        Max=float.MinValue;

    }

    public void AddValue(float v){

        if(v>Max)
            Max=v;
        if(v<Min)
            Min=v;
    }
}
