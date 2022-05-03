using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;


public class GetAllUnitsAtDistanceData
{
    [Serializable]
    public class Base
    {
        public int Index;
        public UnitScript Unit;
        public bool WithSquareDistance;
    }

    // pour tout les jobs
    [Serializable]
    public class DataClass : Base
    {
        
        public DistanceUnitJob DistanceCheck;
        public List<int> TypeMovmentUnit;
        
       

        public void SetValues(UnitScript unitScript, DistanceUnitJob distanceCheck, List<int> movmentTypes, bool withSquareDistance)
        {
            Unit = unitScript;
            Index = unitScript.DistanceUnitsResults.UnitsResults.Count;
 
            DistanceCheck = distanceCheck;
            TypeMovmentUnit = movmentTypes;

            WithSquareDistance = withSquareDistance;
        }

    }
    
    public struct NeighbourCells
    {
        public int3 DistanceLinesCell;
        public int BaseUnitID;
        public int3 LinesPosition;
        
 
        public NeighbourCells (int3 distanceIDCell,  int baseUnitID, int3 linesPosition)
        {
            DistanceLinesCell = distanceIDCell;
            BaseUnitID= baseUnitID;
            LinesPosition = linesPosition;
        }
    }
    
    
    public struct UnitsAtDistance
    {
        public bool WithSquareDistance;
        public float DistanceSquare;
        public float3 BaseUnitPosition;
        public UnitsAtDistance(float distanceSquare, float3 baseUnitPosition, bool withSquareDistance)
        {
            DistanceSquare = distanceSquare;
            BaseUnitPosition = baseUnitPosition;
            WithSquareDistance = withSquareDistance;
        }
    }

  

    
    
    
}





