using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class DestinationSquad
{
    public float3 Position;
    public int IndexMovment;
    public bool ReachedDestination;
    public bool FirstUnitReachedDestination;

   public DestinationSquad(float3 position, int indexMovment)
    {
        Position = position;
        IndexMovment = indexMovment;
        FirstUnitReachedDestination = false;
        ReachedDestination = false; 
    }
}
