using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class UnitJobsManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    public List<UnitJobData.UnitsDistance> DistanceUnitsData;
    public List<UnitJobData.UnitsBool> UnitsBoolData;
    public List<UnitJobData.UnitsNeighboursCell> UnitsNeighboursCellData;
    public List<UnitJobData.TransformUnits> TransformUnitsData;

    

    private void OnValidate()
    {
        
    }
    
    // trier pour avoir le plus proche 
    
    //take damage
    [BurstCompile]
    public struct GetUnitsNeighboursCellJob : IJobParallelFor
    {
        public void Execute(int index)
        {
            throw new NotImplementedException();
        }
    }
    [BurstCompile]
    public struct GetUnitsWithBoolJob : IJobParallelFor
    {
        public void Execute(int index)
        {
            throw new NotImplementedException();
        }
    }
    
    [BurstCompile]
    public struct GetUnitsAtDistanceJob : IJobParallelFor
    {
        public void Execute(int index)
        {
            throw new NotImplementedException();
        }
    }
    
    [BurstCompile]
    public struct SetTransformUnitsJob : IJobParallelForTransform
    {
        public void Execute(int index, TransformAccess transform)
        {
            throw new NotImplementedException();
        }
    }
    
 
    




 
}
