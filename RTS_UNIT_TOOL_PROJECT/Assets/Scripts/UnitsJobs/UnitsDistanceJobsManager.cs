using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Rendering;

public class UnitsDistanceJobsManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    public List<UnitDistanceJobData.UnitsDistanceClass> DistanceUnitsData;
    List<UnitScript> _unitsMovment = new List<UnitScript>();
    [SerializeField] private int _jobSizeNeighbours;
    [SerializeField] private int _jobSizeDistance;
    public static UnitsDistanceJobsManager Instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);    // Suppression d'une instance précédente (sécurité...sécurité...)
 
        Instance = this;
    }


    private void OnDrawGizmos()
    {
    }

    public void OnUpdate()
    {
        if (DistanceUnitsData.Count == 0) return;
        JobHandle _handle = new JobHandle();
        _unitsMovment.Clear();

        SetNeighboursCellJob(out NativeArray<UnitDistanceJobData.NeighbourCells> _neighboursCellDatas);
        int neighboursCellIDCount = 0;
        NativeArray<int> _cellIDMinCounts = new NativeArray<int>(DistanceUnitsData.Count, Allocator.TempJob);
        for (int i = 0; i < DistanceUnitsData.Count; i++)
        {
            _cellIDMinCounts[i] = neighboursCellIDCount;
            neighboursCellIDCount += DistanceUnitsData[i].DistanceCheck.Area;
        }

        NativeArray<int> _AllneighbourCellsID = new NativeArray<int>(neighboursCellIDCount, Allocator.TempJob);
        GetNeighboursCellJob _getNeighboursCellJob = new GetNeighboursCellJob
        {
            AllDatas = _neighboursCellDatas,
            NeighbourCellsID = _AllneighbourCellsID,
            CellIDMinCounts = _cellIDMinCounts,
            GridLinesCount = _gridManager.CellCount,
            GridFactors = _gridManager.CellFactor
        };
        _handle = _getNeighboursCellJob.Schedule(DistanceUnitsData.Count, _jobSizeNeighbours);
        _handle.Complete();

        _gridManager.Grid[DistanceUnitsData[0].BaseUnitID].Color = Color.yellow;
        GetNeighboursCells(_getNeighboursCellJob, out List<List<int>> _finalNeighbourCellsID);

        _neighboursCellDatas.Dispose();
        _cellIDMinCounts.Dispose();
        _AllneighbourCellsID.Dispose();

        NativeArray<MinMaxIndex> _minMaxCountUnits;
        NativeArray<float3> _unitsPosition;
        GetUnitsWithMovmentType(_finalNeighbourCellsID, out _unitsMovment, out _unitsPosition, out _minMaxCountUnits);
        NativeArray<bool> _inDistanceArray = new NativeArray<bool>(_unitsPosition.Length, Allocator.TempJob);
        NativeArray<UnitDistanceJobData.UnitsAtDistance> _unitsAtDistanceDatas;
        SetUnitsAtDistanceJob(out _unitsAtDistanceDatas);

        GetUnitsAtDistanceJob _getUnitsAtDistanceJob = new GetUnitsAtDistanceJob()
        {
            AllDatas = _unitsAtDistanceDatas,
            UnitsPosition = _unitsPosition,
            InDistanceArray = _inDistanceArray,
            MinMaxIndexArray = _minMaxCountUnits,
        };
        _handle = _getUnitsAtDistanceJob.Schedule(DistanceUnitsData.Count, _jobSizeDistance);
        _handle.Complete();
       

GetUnitsAtDistance(_inDistanceArray, _minMaxCountUnits); 
_unitsAtDistanceDatas.Dispose();
        _minMaxCountUnits.Dispose();
        _inDistanceArray.Dispose();
        _unitsPosition.Dispose();
        DistanceUnitsData.Clear();
    }

    void GetUnitsAtDistance(NativeArray<bool> inDistanceArray, NativeArray<MinMaxIndex> minMaxIndices)
    {
       
        int maxCount = 0;
        for (int i = 0; i < inDistanceArray.Length; i++)
        {
            if (minMaxIndices[maxCount].MaxIndex == i)
            {
                maxCount++;
            }
            if (inDistanceArray[i])
            {
                DistanceUnitsData[maxCount].Unit.Results.UnitsResults[DistanceUnitsData[maxCount].Index].Units.Add(_unitsMovment[i]);
             
            }   
           
        }

        for (int i = 0; i < DistanceUnitsData.Count; i++)
        {
            DistanceUnitsData[i].Unit.Results.UnitsResults[DistanceUnitsData[i].Index].IsResult = true;
            
        }
    }

    void GetNeighboursCells(GetNeighboursCellJob getNeighboursCellJob, out List<List<int>> finalNeighbourCellsID)
    {
        finalNeighbourCellsID = new List<List<int>>();

    
        int w = -1;
        int j = 0;
        for (int i = 0; i < getNeighboursCellJob.NeighbourCellsID.Length; i++)
        {
            if (j != getNeighboursCellJob.CellIDMinCounts.Length)
            {
                if (i == getNeighboursCellJob.CellIDMinCounts[j])
                {
                    j++;
                    w++;
                    finalNeighbourCellsID.Add(new List<int>());
                }
            }

            finalNeighbourCellsID[w].Add(getNeighboursCellJob.NeighbourCellsID[i]);
        }
    }

    // trier pour avoir le plus proche 

    //take damage

    private void GetUnitsWithMovmentType(List<List<int>> NeighbourCellsID,
        out List<UnitScript> unitsMovment, out NativeArray<float3> unitsPosition,
        out NativeArray<MinMaxIndex> minCountUnits)
    {
        
        unitsMovment = new List<UnitScript>();
        List<float3> unitsPositionList = new List<float3>();
        List<UnitScript> unitCellsAdd = new List<UnitScript>();
        minCountUnits = new NativeArray<MinMaxIndex>(DistanceUnitsData.Count, Allocator.TempJob);
        for (int i = 0; i < DistanceUnitsData.Count; i++)
        {
            MinMaxIndex _currentMinMaxIndex = minCountUnits[i];
           
                       _currentMinMaxIndex.MinIndex = unitsPositionList.Count;
            
            for (int j = 0; j < DistanceUnitsData[i].TypeMovmentUnit.Count; j++)
            {
                for (int k = 0; k < NeighbourCellsID[i].Count; k++)
                {
                    var _allUnits = _gridManager.Grid[NeighbourCellsID[i][k]].AllUnits;
                    for (int l = 0;
                        l < _allUnits.Count;
                        l++)
                    {
                        if (DistanceUnitsData[i].TypeMovmentUnit[j] == (int) _allUnits[l].MovmentType)
                        {
                          
                            for (int m = 0; m < _allUnits[l].Units.Count; m++)
                            {
                                if(_allUnits[l].Units[m] == DistanceUnitsData[i].Unit || unitCellsAdd.Contains(_allUnits[l].Units[m]) )
                                    continue;
                                
                               Debug.Log(_allUnits[l].Units[m]);
                                    unitsMovment.Add(_allUnits[l].Units[m]);
                                    unitCellsAdd.Add(_allUnits[l].Units[m]);
                                unitsPositionList.Add(_allUnits[l].Units[m].transform.position);
                            }
                        }
                    
                    }  
                   
                    
                    unitCellsAdd.Clear();
                }
            }
            
        
            _currentMinMaxIndex.MaxIndex = unitsPositionList.Count;
            minCountUnits[i] = _currentMinMaxIndex;
        
       
            
        }

        for (int i = 0; i < DistanceUnitsData[i].TypeMovmentUnit.Count; i++)
        {
            Debug.Log(i + DistanceUnitsData[i].BaseUnitID + "   " + DistanceUnitsData[i].TypeMovmentUnit[i]);
        }

        for (int j = 0; j < DistanceUnitsData.Count; j++)
                    {
                        
                        for (int k = minCountUnits[j].MinIndex; k < minCountUnits[j].MaxIndex; k++)
                        {
                            Debug.Log(j +"   "+DistanceUnitsData[j].BaseUnitID+"    " + "" +
                                      unitsMovment[k].Cell.ID);
                        }
                      }
        
unitsPosition = new NativeArray<float3>(unitsPositionList.Count, Allocator.TempJob);
for (int i = 0; i < unitsPositionList.Count; i++)
{
    unitsPosition[i] = unitsPositionList[i];
}
    }

    private void SetNeighboursCellJob(out NativeArray<UnitDistanceJobData.NeighbourCells> neighboursCellDatas)
    {
        neighboursCellDatas =
            new NativeArray<UnitDistanceJobData.NeighbourCells>(DistanceUnitsData.Count, Allocator.TempJob);
        
        for (int i = 0; i < DistanceUnitsData.Count; i++)
        {
            neighboursCellDatas[i] = new UnitDistanceJobData.NeighbourCells(
                DistanceUnitsData[i].DistanceCheck.LinesCell,
                DistanceUnitsData[i].BaseUnitID, DistanceUnitsData[i].LinesPosition);
        }
    }

    private void SetUnitsAtDistanceJob(out NativeArray<UnitDistanceJobData.UnitsAtDistance> unitsAtDistanceDatas)
    {
        unitsAtDistanceDatas =
            new NativeArray<UnitDistanceJobData.UnitsAtDistance>(DistanceUnitsData.Count, Allocator.TempJob);
        for (int i = 0; i < DistanceUnitsData.Count; i++)
        {
            unitsAtDistanceDatas[i]
                 = new UnitDistanceJobData.UnitsAtDistance(DistanceUnitsData[i].DistanceCheck.Square, DistanceUnitsData[i].BaseUnitPosition);
        }
    }

    [BurstCompile]
    public struct GetNeighboursCellJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<UnitDistanceJobData.NeighbourCells> AllDatas;
        [ReadOnly] public int3 GridFactors;
        [ReadOnly] public int3 GridLinesCount;


        [NativeDisableParallelForRestriction] [WriteOnly]
        public NativeArray<int> NeighbourCellsID;

        [ReadOnly] public NativeArray<int> CellIDMinCounts;


        public void Execute(int index)
        {
            var _data = AllDatas[index];

            int3 _minDistanceLines = _data.DistanceLinesCell;
            int3 _maxDistanceLines = _data.DistanceLinesCell;

            if ((_data.LinesPosition.x + _data.DistanceLinesCell.x) >= GridLinesCount.x)
                _maxDistanceLines.x -= _data.LinesPosition.x + _data.DistanceLinesCell.x;
            if ((_data.LinesPosition.y + _data.DistanceLinesCell.y) >= GridLinesCount.y)
                _maxDistanceLines.y -= _data.LinesPosition.y + _data.DistanceLinesCell.y;
            if ((_data.LinesPosition.z + _data.DistanceLinesCell.z) >= GridLinesCount.z)
                _maxDistanceLines.z -= _data.LinesPosition.z + _data.DistanceLinesCell.z;

            if ((_data.LinesPosition.x - _data.DistanceLinesCell.x) <= 0)
                _minDistanceLines.x += _data.LinesPosition.x - _data.DistanceLinesCell.x;
            if ((_data.LinesPosition.y - _data.DistanceLinesCell.y) <= 0)
                _minDistanceLines.y += _data.LinesPosition.y - _data.DistanceLinesCell.y;
            if ((_data.LinesPosition.z - _data.DistanceLinesCell.z) <= 0)
                _minDistanceLines.z += _data.LinesPosition.z - _data.DistanceLinesCell.z;


            _maxDistanceLines *= GridFactors;
            _minDistanceLines *= GridFactors;


            int _xMin = _data.BaseUnitID -
                        _minDistanceLines.x;
            int _xMax = _data.BaseUnitID +
                        _maxDistanceLines.x;

            int minCount = CellIDMinCounts[index];

            for (int x = _xMin; x < _xMax + 1; x += GridFactors.x)
            {
                int _yMin = x - _minDistanceLines.y;
                int _yMax = x + _maxDistanceLines.y;


                for (int y = _yMin; y < _yMax + 1; y += GridFactors.y)
                {
                    int _zMin = y - _minDistanceLines.z;
                    int _zMax = y + _maxDistanceLines.z;

                    for (int z = _zMin; z < _zMax + 1; z += GridFactors.z)
                    {
                        NeighbourCellsID[minCount] = z;
                        minCount++;
                    }
                }
            }
        }
    }


    [BurstCompile]
    public struct GetUnitsAtDistanceJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<UnitDistanceJobData.UnitsAtDistance> AllDatas;
        [ReadOnly]
        public NativeArray<float3> UnitsPosition;
        [NativeDisableParallelForRestriction]
        [WriteOnly]
        public NativeArray<bool> InDistanceArray;
        [ReadOnly]
        public NativeArray<MinMaxIndex> MinMaxIndexArray;

        public void Execute(int index)
        {
       
            var _data = AllDatas[index];
            
            for (int i = MinMaxIndexArray[index].MinIndex; i < MinMaxIndexArray[index].MaxIndex; i++)
            {
                if (Vector3.SqrMagnitude(UnitsPosition[i] - _data.BaseUnitPosition) <= _data.DistanceSquare)
                {
                    print(Vector3.Distance(UnitsPosition[i], _data.BaseUnitPosition));
                    InDistanceArray[i] = true;
                }
    
            
            }
        }
    }

}