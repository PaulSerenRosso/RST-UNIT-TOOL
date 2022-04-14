using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using Unity.Mathematics;


public class UnitJobData
{
    [Serializable]
    public class Base
    {
        public UnitModule module;
        public int index;
    }

    // pour tout les jobs
    public class UnitsDistance : Base
    {
        public float3 BaseUnitPosition;
        public DistanceCell DistanceCheck;
        public List<int> TypeMovmentUnit;
        public List<List<int>> CellTypeMovment;
    }
    
    public struct NeighbourCells
    {
        public float3 DistanceCell;
        public float3 BaseUnitPosition;
        public NativeArray<int> NeighbourCellsID;

        public void SetValues(float3 distanceCell,  float3 baseUnitPosition)
        {
            DistanceCell = distanceCell;
            BaseUnitPosition = baseUnitPosition;
        }
    }
    
    public struct UnitsWithMovmentType
    {
        public NativeArray<NativeArray<int>> TypeMovmentCells;
        public NativeArray<int> TypeMovmentUnit;
        public NativeArray<int> indexUnit;
        public void SetValues()
        {
            
        }
    }
    
    public struct UnitsAtDistance
    {
        public float Distance;
        public float3 BaseUnitPosition; 
        public NativeArray<float3> UnitsPosition;
        public NativeArray<int> indexUnit;
        public void SetValues()
        {
            
        }
    }
    
    
    
    
    
}





