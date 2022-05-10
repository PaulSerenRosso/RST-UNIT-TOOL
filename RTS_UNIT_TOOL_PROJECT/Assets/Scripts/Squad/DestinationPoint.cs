using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class DestinationPoint
{
    public float3 Position;
    public int IndexMovment;
    public bool ReachedDestination;
    public bool FirstUnitReachedDestination;

   public DestinationPoint(float3 position, int indexMovment)
    {
        Position = position;
        IndexMovment = indexMovment;
        FirstUnitReachedDestination = false;
        ReachedDestination = false; 
    }
}
