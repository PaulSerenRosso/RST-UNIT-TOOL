using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class DestinationPosition
{
    public float3 Position;
    public int IndexMovment;

    public DestinationPosition(float3 position, int indexMovment)
    {
        Position = position;
        IndexMovment = indexMovment;
    }
}