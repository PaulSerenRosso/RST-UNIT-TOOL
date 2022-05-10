using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
[Serializable]
public class DestinationUnit : DestinationPosition
{
    public List<UnitScript> Units; 
    public DestinationUnit(float3 position, int indexMovment, List<UnitScript> units) : base(position, indexMovment)
    {
        Units = units;
    }
}
