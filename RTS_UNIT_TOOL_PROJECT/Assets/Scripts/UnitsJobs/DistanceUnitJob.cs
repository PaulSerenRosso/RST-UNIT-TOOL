using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct DistanceUnitJob
{
    public float Square;
    public int3 LinesCell;
    public int Area;
}