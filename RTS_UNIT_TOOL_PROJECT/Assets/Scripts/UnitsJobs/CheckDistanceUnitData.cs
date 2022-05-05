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
        public UnitScript CheckUnit;
        public bool InNeighbourCells;
    }
    public struct UnitsAtDistance
    {
        
        public float3 CheckUnitPosition;
        public float SquareDistance;
        public float3 BaseUnitPosition;
        public bool InDistance;
        public float SquareDistanceCheckUnit;
        public UnitsAtDistance(float distanceSquare, float3 baseUnitPosition, float3 checkUnitPosition)
        {
            SquareDistance = distanceSquare;
            BaseUnitPosition = baseUnitPosition;
            CheckUnitPosition = checkUnitPosition;
            InDistance = false;
            SquareDistanceCheckUnit = -1;
        }
    }
    
    
}
