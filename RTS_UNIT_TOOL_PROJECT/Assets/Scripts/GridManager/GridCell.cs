using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class GridCell
{
    public int ID;
    public float3 MinPosition;
    public float3 MaxPosition;
    public float3 CenterPosition;
    public List<UnitMovmentList> AllUnits;
    public int3 LinesPosition;
    [HideInInspector] public Color Color;

    public bool TryGetIndexList(UnitMovmentType unitMovmentList, out int index)
    {
        for (int i = 0; i < AllUnits.Count; i++)
        {
            if (unitMovmentList == AllUnits[i].MovmentType)
            {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }
}

