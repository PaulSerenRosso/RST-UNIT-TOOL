using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class DistanceCellsClass
{
    public float Base;
    public DistanceUnitJob DistanceJob;
    
    public void ConvertDistanceToDistanceCell(GridManager gridManager)
    {
        DistanceJob.Square = Base * Base;
        DistanceJob.LinesCell = new int3(Mathf.Max(Mathf.RoundToInt(Base / gridManager.SizeCells.x),1) ,Mathf.Max(Mathf.RoundToInt(Base / gridManager.SizeCells.y),1)
             ,Mathf.Max(Mathf.RoundToInt(Base / gridManager.SizeCells.z),1)
             );
      
        DistanceJob.Area = (1+2*DistanceJob.LinesCell.x) * (1+2*DistanceJob.LinesCell.y) * (1+2*DistanceJob.LinesCell.z);

    }

    public void SetValues(float currentBase, DistanceUnitJob distanceUnitJob )
    {
        Base = currentBase;
        DistanceJob = distanceUnitJob;
    }
}