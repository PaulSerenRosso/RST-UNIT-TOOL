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
        DistanceJob.LinesCell = new int3(Mathf.RoundToInt(Base / gridManager.SizeCells.x) ,
            Mathf.RoundToInt(Base / gridManager.SizeCells.y) ,
            Mathf.RoundToInt(Base / gridManager.SizeCells.z) );
        DistanceJob.Area = (1+2*DistanceJob.LinesCell.x) * (1+2*DistanceJob.LinesCell.y) * (1+2*DistanceJob.LinesCell.z);

    }
}