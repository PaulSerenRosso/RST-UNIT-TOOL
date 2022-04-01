using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class GridCell
{
    public int id;
     public float3 MinPosition;
    public float3 MaxPosition;
    public float3 CenterPosition;
}
