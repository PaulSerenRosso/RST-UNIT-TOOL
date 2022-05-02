using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Rendering;

public class DistanceUnitsJobsManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    public List<DistanceUnitsJobData.UnitsDistanceClass> DistanceUnitsData;
    List<UnitScript> _unitsMovment = new List<UnitScript>();
    [SerializeField] private int _jobSizeNeighbours;
    [SerializeField] private int _jobSizeDistance;
    public static DistanceUnitsJobsManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject); // Suppression d'une instance précédente (sécurité...sécurité...)

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

        SetNeighboursCellJob(out NativeArray<DistanceUnitsJobData.NeighbourCells> _neighboursCellDatas);
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
        NativeArray<DistanceUnitsJobData.UnitsAtDistance> _unitsAtDistanceDatas;
        List<DistanceUnitsJobData.Base> _unitsAtDistanceBaseDatas;
        GetUnitsWithMovmentType(_finalNeighbourCellsID, out _unitsMovment, out _unitsPosition, out _minMaxCountUnits, out _unitsAtDistanceDatas, out _unitsAtDistanceBaseDatas );
        NativeArray<CheckUnitAtDistance> _inDistanceArray = new NativeArray<CheckUnitAtDistance>(_unitsPosition.Length, Allocator.TempJob);
        
        GetUnitsAtDistanceJob _getUnitsAtDistanceJob = new GetUnitsAtDistanceJob()
        {
            AllDatas = _unitsAtDistanceDatas,
            UnitsPosition = _unitsPosition,
            InDistanceArray = _inDistanceArray,
            MinMaxIndexArray = _minMaxCountUnits,
        };
        _handle = _getUnitsAtDistanceJob.Schedule(_unitsAtDistanceDatas.Length, _jobSizeDistance);
        _handle.Complete();


        GetUnitsAtDistance(_unitsAtDistanceBaseDatas, _inDistanceArray, _minMaxCountUnits);
        _unitsAtDistanceDatas.Dispose();
        _minMaxCountUnits.Dispose();
        _inDistanceArray.Dispose();
        _unitsPosition.Dispose();
        DistanceUnitsData.Clear();
    }

    void GetUnitsAtDistance(List<DistanceUnitsJobData.Base> baseDatas,  NativeArray<CheckUnitAtDistance> inDistanceArray, NativeArray<MinMaxIndex> minMaxIndices)
    {
        int maxCount = 0;
            
        for (int i = 0; i < inDistanceArray.Length; i++)
        {
     
            if (minMaxIndices[maxCount].MaxIndex == i)
            {
                maxCount++;
            }

     
            if (inDistanceArray[i].InDistance)
            {
                if (baseDatas[maxCount].WithSquareDistance)
                {
                    UnitWithDistanceAmount unitWithDistanceAmount = new UnitWithDistanceAmount();
                    unitWithDistanceAmount.SquareDistance = inDistanceArray[i].SquareDistance;
                    unitWithDistanceAmount.Unit = _unitsMovment[i];
                    baseDatas[maxCount].Unit.DistanceUnitsResults.UnitsResultAmounts[baseDatas[maxCount].Index].UnitsWithAmount.Add(unitWithDistanceAmount); 
                }
                else
                {
                    baseDatas[maxCount].Unit.DistanceUnitsResults.UnitsResults[baseDatas[maxCount].Index].Units
                        .Add(_unitsMovment[i]);
                }
             ;
            }
        }

        for (int i = 0; i < DistanceUnitsData.Count; i++)
        {
            DistanceUnitsData[i].Unit.DistanceUnitsResults.UnitsResults[DistanceUnitsData[i].Index].IsResult = true;
        }
    }

    void GetNeighboursCells( GetNeighboursCellJob getNeighboursCellJob, out List<List<int>> finalNeighbourCellsID)
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
        out NativeArray<MinMaxIndex> minCountUnits,
        out NativeArray<DistanceUnitsJobData.UnitsAtDistance> unitsAtDistanceDatas, out List<DistanceUnitsJobData.Base> baseDatas)
    {
        
        List<MinMaxIndex> minMaxIndicesList = new List<MinMaxIndex>();
        unitsMovment = new List<UnitScript>();
        List<float3> unitsPositionList = new List<float3>();
        List<UnitScript> unitCellsAdd = new List<UnitScript>();
        List<DistanceUnitsJobData.UnitsAtDistance> currentUnitsAtDistance =
            new List<DistanceUnitsJobData.UnitsAtDistance>();
        baseDatas = new List<DistanceUnitsJobData.Base>();
        for (int i = 0; i < DistanceUnitsData.Count; i++)
        {
            MinMaxIndex _currentMinMaxIndex = new MinMaxIndex();

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
                                if (_allUnits[l].Units[m] == DistanceUnitsData[i].Unit ||
                                    unitCellsAdd.Contains(_allUnits[l].Units[m]) )
                                    continue;
                                
                                unitsMovment.Add(_allUnits[l].Units[m]);
                                unitCellsAdd.Add(_allUnits[l].Units[m]);
                         //       Debug.Log(DistanceUnitsData[i].Index + "   "+ DistanceUnitsData[i].Unit.Cell.ID +"   "+ _allUnits[l].Units[m].Cell.ID + "distance "+Vector3.Distance(DistanceUnitsData[i].Unit.transform.position,_allUnits[l].Units[m].transform.position ));
                                unitsPositionList.Add(_allUnits[l].Units[m].transform.position);
                            }
                        }
                    }


                    unitCellsAdd.Clear();
                }
            }

            _currentMinMaxIndex.MaxIndex = unitsPositionList.Count;
            if (_currentMinMaxIndex.MaxIndex !=_currentMinMaxIndex.MinIndex )
            {
                minMaxIndicesList.Add(_currentMinMaxIndex);
                DistanceUnitsJobData.UnitsAtDistance unitsAtDistance =
                    new DistanceUnitsJobData.UnitsAtDistance(DistanceUnitsData[i].DistanceCheck.Square,
                        DistanceUnitsData[i].BaseUnitPosition, DistanceUnitsData[i].WithSquareDistance );
                currentUnitsAtDistance.Add(unitsAtDistance);
                DistanceUnitsJobData.Base currentBase = new DistanceUnitsJobData.Base();
                currentBase.Index = DistanceUnitsData[i].Index;
                currentBase.Unit = DistanceUnitsData[i].Unit;
                baseDatas.Add(currentBase);
            }
        }

        unitsAtDistanceDatas =
            new NativeArray<DistanceUnitsJobData.UnitsAtDistance>(currentUnitsAtDistance.Count, Allocator.TempJob);
        minCountUnits = new NativeArray<MinMaxIndex>(currentUnitsAtDistance.Count, Allocator.TempJob);
        for (int i = 0; i < currentUnitsAtDistance.Count; i++)
        {
            unitsAtDistanceDatas[i] = currentUnitsAtDistance[i];
            minCountUnits[i] = minMaxIndicesList[i];
        }
        
        
        unitsPosition = new NativeArray<float3>(unitsPositionList.Count, Allocator.TempJob);
        for (int i = 0; i < unitsPositionList.Count; i++)
        {
            unitsPosition[i] = unitsPositionList[i];
        }
    }

    private void SetNeighboursCellJob(out NativeArray<DistanceUnitsJobData.NeighbourCells> neighboursCellDatas)
    {
        neighboursCellDatas =
            new NativeArray<DistanceUnitsJobData.NeighbourCells>(DistanceUnitsData.Count, Allocator.TempJob);

        for (int i = 0; i < DistanceUnitsData.Count; i++)
        {
            neighboursCellDatas[i] = new DistanceUnitsJobData.NeighbourCells(
                DistanceUnitsData[i].DistanceCheck.LinesCell,
                DistanceUnitsData[i].BaseUnitID, DistanceUnitsData[i].LinesPosition);
        }
    }


    [BurstCompile]
    public struct GetNeighboursCellJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<DistanceUnitsJobData.NeighbourCells> AllDatas;
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
        [ReadOnly] public NativeArray<DistanceUnitsJobData.UnitsAtDistance> AllDatas;
        [ReadOnly] public NativeArray<float3> UnitsPosition;

        [NativeDisableParallelForRestriction] [WriteOnly]
        public NativeArray<CheckUnitAtDistance> InDistanceArray;

        [ReadOnly] public NativeArray<MinMaxIndex> MinMaxIndexArray;

        public void Execute(int index)
        {
            var _data = AllDatas[index];

            for (int i = MinMaxIndexArray[index].MinIndex; i < MinMaxIndexArray[index].MaxIndex; i++)
            {
                CheckUnitAtDistance checkUnit = InDistanceArray[i]; 
                float squareDistance = Vector3.SqrMagnitude(UnitsPosition[i] - _data.BaseUnitPosition);
                if (squareDistance <= _data.DistanceSquare)
                {
                    checkUnit.InDistance= true;
                   if (_data.WithSquareDistance) checkUnit.SquareDistance = squareDistance;
                }
                InDistanceArray[i] = checkUnit;
            }
        }
    }

    public struct CheckUnitAtDistance
    {
        public bool InDistance;
        public float SquareDistance;
    }
}