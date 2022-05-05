using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TerrainYDistance
{
    public UnitMovmentType Type;
    public float YDistance;

    public TerrainYDistance(UnitMovmentType type, float yDistance)
    {
        Type = type;
        YDistance = yDistance;
    }
}
