using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class UnitJobsManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    public List<UnitJobData.UnitsDistance> DistanceUnitsData;
    


    // premier job 
    // Check les cases proches 
    // avoir les distances transformer en line count
    // centre + line count distance line count
    // centre - line count distance 
    
    // deusième job go l'utiliser pour avoir l'update de 
    // mettre une id avec une list d'int 
    // comparer avec les int possibles 
    // et ajouter 
    // Checks les cases qui possèdes la bonne liste
    
    //3 ème job 
    // il me faut le transform du centre et des untiés que je puisse comparer
    // Checks dans cette list d'unité lequelq on une distance 



// check si un truc est null si non tu lance un job à partir de la list
    void OnUpdate()
    {
        if(DistanceUnitsData.Count == 0) return;
 
        NativeArray<UnitJobData.NeighbourCells>  _neighboursCellDatas  = new NativeArray<UnitJobData.NeighbourCells>(DistanceUnitsData.Count, Allocator.TempJob);
        for (int i = 0; i < DistanceUnitsData.Count; i++)
        {
            _neighboursCellDatas[i].SetValues(DistanceUnitsData[i].DistanceCheck.LineCount, DistanceUnitsData[i].BaseUnitPosition);
        }

        GetNeighboursCellJob _getNeighboursCellJob = new GetNeighboursCellJob
        {
            gridLinesCount =  _gridManager.CellCount,
            gridBasePosition = _gridManager.transform.position,
            AllDatas = _neighboursCellDatas

        };
        JobHandle _handle= _getNeighboursCellJob.Schedule(DistanceUnitsData.Count, 50);
        _handle.Complete();
        
        _neighboursCellDatas.Dispose();
        DistanceUnitsData.Clear();
    }
    
    // trier pour avoir le plus proche 
    
    //take damage
    [BurstCompile]
    public struct GetNeighboursCellJob : IJobParallelFor
    {
        public NativeArray<UnitJobData.NeighbourCells> AllDatas;
        public float3 gridBasePosition;
        public int3 gridLinesCount;

        public void Execute(int index)
        {
            var data = AllDatas[index];
            float3 _min = (data.BaseUnitPosition - data.DistanceCell - gridBasePosition) / gridLinesCount;
            _min.x = Mathf.FloorToInt(_min.x);
            _min.y = Mathf.FloorToInt(_min.y);
            _min.z = Mathf.FloorToInt(_min.z);
            int3 _finalMin = (int3) _min;
            float3 _max = (data.BaseUnitPosition - data.DistanceCell - gridBasePosition) / gridLinesCount;
            _max.x = Mathf.FloorToInt(_min.x);
            _max.y = Mathf.FloorToInt(_min.y);
            _max.z = Mathf.FloorToInt(_min.z);
            int3 _finalMax = (int3) _max;
            int idCellMax = (_finalMax.x) * gridLinesCount.y * gridLinesCount.z +
                            (_finalMax.y) * gridLinesCount.z + _finalMax.z;
            int idCellMin = (_finalMax.x) * gridLinesCount.y * gridLinesCount.z +
                            (_finalMax.y) * gridLinesCount.z + _finalMax.z;





            AllDatas[index] = data;
        }

    }
    [BurstCompile]
    public struct GetUnitsWithMovmentTypeJob : IJobParallelFor
    {
        public NativeArray<UnitJobData.UnitsWithMovmentType> AllDatas;
        public void Execute(int index)
        {
            throw new NotImplementedException();
        }
    }
    
    [BurstCompile]
    public struct GetUnitsAtDistanceJob : IJobParallelFor
    {
        public NativeArray<UnitJobData.UnitsAtDistance> AllDatas;
        public void Execute(int index)
        {
            throw new NotImplementedException();
        }
    }
    
 
 
    




 
}
