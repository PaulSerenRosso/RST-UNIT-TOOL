using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CheckDistanceUnitData 
{
    public class DataClass
    {
        public int Index;
        public UnitScript Unit;
        public DistanceUnitJob DistanceCheck;
    }
    
    public struct NeighbourCells
    {
        public int3 DistanceLinesCell;
        public int CheckUnitID;
        public int3 CheckUnitLinesPosition;
        
        public NeighbourCells (int3 distanceIDCell,  int baseUnitID, int3 linesPosition)
        {
            DistanceLinesCell = distanceIDCell;
            CheckUnitID= baseUnitID;
            CheckUnitLinesPosition = linesPosition;
        }
    }
    
    public struct UnitsAtDistance
    {
        public float3 CheckUnitPosition;
        public float DistanceSquare;
        public float3 BaseUnitPosition;
        public UnitsAtDistance(float distanceSquare, float3 baseUnitPosition, float3 checkUnitPosition)
        {
            DistanceSquare = distanceSquare;
            BaseUnitPosition = baseUnitPosition;
            CheckUnitPosition = checkUnitPosition;
        }
    }
    
    
}
