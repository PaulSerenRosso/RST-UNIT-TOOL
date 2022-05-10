using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine.Profiling;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Rendering;

public class DistanceUnitsJobsManager : MonoBehaviour
{
    #region GetAllUnitsAtDistance

    [Header("GetAllUnitsAtDistance")] [SerializeField]
    private GridManager _gridManager;

    public List<GetAllUnitsAtDistanceData.DataClass> GetUnitsData;
    [SerializeField] private int _jobSizeNeighbours;
    [SerializeField] private int _jobSizeDistance;

    #endregion


    #region CheckDistanceUnit

    [Header(" CheckDistanceUnit")]
    public List<CheckDistanceUnitData.DataClass> CheckUnitData = new List<CheckDistanceUnitData.DataClass>();

    [SerializeField] private int _jobSizeCheckNeighbours;
    [SerializeField] private int _jobSizeCheckDistance;

    #endregion

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

    void SetNeighboursCellCheckJob(out NativeArray<GetAllUnitsAtDistanceData.NeighbourCells> neighboursCellDatas,
        out NativeArray<int> cellIDMinCounts, out int neighboursCellIDCount)
    {
        neighboursCellDatas =
            new NativeArray<GetAllUnitsAtDistanceData.NeighbourCells>(CheckUnitData.Count, Allocator.TempJob);
        cellIDMinCounts = new NativeArray<int>(CheckUnitData.Count, Allocator.TempJob);
        for (int i = 0; i < CheckUnitData.Count; i++)
        {
            neighboursCellDatas[i] = new GetAllUnitsAtDistanceData.NeighbourCells(
                CheckUnitData[i].DistanceCheck.LinesCell,
                CheckUnitData[i].Unit.Cell.ID, CheckUnitData[i].Unit.Cell.LinesPosition);
        }

        neighboursCellIDCount = 0;

        for (int i = 0; i < CheckUnitData.Count; i++)
        {
            cellIDMinCounts[i] = neighboursCellIDCount;
            neighboursCellIDCount += CheckUnitData[i].DistanceCheck.Area;
        }
    }

    void SetCheckUnitAtDistanceJob(GetNeighboursCellJob getNeighboursCellJob,
        out NativeArray<CheckDistanceUnitData.UnitsAtDistance> unitAtDistanceDatas)
    {
        List<CheckDistanceUnitData.UnitsAtDistance> _unitAtDistanceDatasList =
            new List<CheckDistanceUnitData.UnitsAtDistance>();

        int count = 0;
        for (int i = 0; i < getNeighboursCellJob.NeighbourCellsID.Length; i++)
        {
            if (count != getNeighboursCellJob.CellIDMinCounts.Length - 1)
            {
                if (getNeighboursCellJob.CellIDMinCounts[count + 1] == i)
                {
                    count++;
                }
            }


            if (getNeighboursCellJob.NeighbourCellsID[i] == CheckUnitData[count].CheckUnit.Cell.ID)
            {
                _unitAtDistanceDatasList.Add(new CheckDistanceUnitData.UnitsAtDistance(
                    CheckUnitData[count].DistanceCheck.Square, CheckUnitData[count].Unit.transform
                        .position, CheckUnitData[count].CheckUnit.transform.position));
                CheckUnitData[count].InNeighbourCells = true;

                if (count != getNeighboursCellJob.CellIDMinCounts.Length - 1)
                    i = getNeighboursCellJob.CellIDMinCounts[count + 1] - 1;
            }
        }

        for (int i = CheckUnitData.Count - 1; i > -1; i--)
        {
            if (!CheckUnitData[i].InNeighbourCells)
            {
                CheckUnitData.Remove(CheckUnitData[i]);
            }
        }

        unitAtDistanceDatas =
            new NativeArray<CheckDistanceUnitData.UnitsAtDistance>(_unitAtDistanceDatasList.Count,
                Allocator.TempJob);

        for (int i = 0; i < _unitAtDistanceDatasList.Count; i++)
        {
            unitAtDistanceDatas[i] = _unitAtDistanceDatasList[i];
        }
    }

    public void CheckDistanceUnit()
    {
        if (CheckUnitData.Count == 0) return;
        JobHandle _handle = new JobHandle();


        SetNeighboursCellCheckJob(out NativeArray<GetAllUnitsAtDistanceData.NeighbourCells> _neighboursCellDatas,
            out NativeArray<int> _cellIDMinCounts, out int _neighboursCellIDCount);

        NativeArray<int> _AllneighbourCellsID = new NativeArray<int>(_neighboursCellIDCount, Allocator.TempJob);
        GetNeighboursCellJob _getNeighboursCellJob = new GetNeighboursCellJob
        {
            AllDatas = _neighboursCellDatas,
            NeighbourCellsID = _AllneighbourCellsID,
            CellIDMinCounts = _cellIDMinCounts,
            GridLinesCount = _gridManager.CellCount,
            GridFactors = _gridManager.CellFactor
        };
        _handle = _getNeighboursCellJob.Schedule(CheckUnitData.Count, _jobSizeCheckNeighbours);
        _handle.Complete();

        SetCheckUnitAtDistanceJob(_getNeighboursCellJob,
            out NativeArray<CheckDistanceUnitData.UnitsAtDistance> _unitAtDistanceDatas);

        CheckUnitAtDistanceJob _checkUnitAtDistanceJob = new CheckUnitAtDistanceJob()
        {
            AllDatas = _unitAtDistanceDatas
        };
        _handle = _checkUnitAtDistanceJob.Schedule(_unitAtDistanceDatas.Length, _jobSizeCheckDistance);
        _handle.Complete();

        for (int i = 0; i < CheckUnitData.Count; i++)
        {
            DistanceUnitsJobResults.CheckUnitAtDistanceClass checkUnitAtDistanceClass = CheckUnitData[i].Unit
                .DistanceUnitsResults.CheckDistanceUnitResults[CheckUnitData[i].Index];
            checkUnitAtDistanceClass.InDistance = _checkUnitAtDistanceJob.AllDatas[i].InDistance;
            checkUnitAtDistanceClass.SquareDistance = _checkUnitAtDistanceJob.AllDatas[i].SquareDistanceCheckUnit;
            CheckUnitData[i].Unit
                .DistanceUnitsResults.CheckDistanceUnitResults[CheckUnitData[i].Index] = checkUnitAtDistanceClass;
        }

        _cellIDMinCounts.Dispose();
        _neighboursCellDatas.Dispose();
        _unitAtDistanceDatas.Dispose();
        _AllneighbourCellsID.Dispose();
        CheckUnitData.Clear();
    }


    public void GetAllUnitsAtDistance()
    {
        if (GetUnitsData.Count == 0) return;
        JobHandle _handle = new JobHandle();

        SetNeighboursCellJob(out NativeArray<GetAllUnitsAtDistanceData.NeighbourCells> _neighboursCellDatas);
        int neighboursCellIDCount = 0;
        NativeArray<int> _cellIDMinCounts = new NativeArray<int>(GetUnitsData.Count, Allocator.TempJob);
        for (int i = 0; i < GetUnitsData.Count; i++)
        {
            _cellIDMinCounts[i] = neighboursCellIDCount;
            neighboursCellIDCount += GetUnitsData[i].DistanceCheck.Area;
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
        _handle = _getNeighboursCellJob.Schedule(GetUnitsData.Count, _jobSizeNeighbours);
        _handle.Complete();

    
        GetNeighboursCells(_getNeighboursCellJob, out List<List<int>> _finalNeighbourCellsID);

        _neighboursCellDatas.Dispose();
        _cellIDMinCounts.Dispose();
        _AllneighbourCellsID.Dispose();

        NativeArray<MinMaxIndex> _minMaxCountUnits;
        NativeArray<float3> _unitsPosition;
        NativeArray<GetAllUnitsAtDistanceData.UnitsAtDistance> _unitsAtDistanceDatas;
        List<UnitScript> _unitsMovment;

        GetUnitsWithMovmentType(_finalNeighbourCellsID, out _unitsMovment, out _unitsPosition, out _minMaxCountUnits,
            out _unitsAtDistanceDatas);
        NativeArray<CheckUnitAtDistance> _inDistanceArray =
            new NativeArray<CheckUnitAtDistance>(_unitsPosition.Length, Allocator.TempJob);

        GetUnitsAtDistanceJob _getUnitsAtDistanceJob = new GetUnitsAtDistanceJob()
        {
            AllDatas = _unitsAtDistanceDatas,
            UnitsPosition = _unitsPosition,
            InDistanceArray = _inDistanceArray,
            MinMaxIndexArray = _minMaxCountUnits,
        };
        _handle = _getUnitsAtDistanceJob.Schedule(GetUnitsData.Count, _jobSizeDistance);
        _handle.Complete();


        GetUnitsAtDistance(_unitsMovment, _inDistanceArray, _minMaxCountUnits);
        _unitsAtDistanceDatas.Dispose();
        _minMaxCountUnits.Dispose();
        _inDistanceArray.Dispose();
        _unitsPosition.Dispose();

        GetUnitsData.Clear();
    }

    void GetUnitsAtDistance(List<UnitScript> unitsMovment,
        NativeArray<CheckUnitAtDistance> inDistanceArray, NativeArray<MinMaxIndex> minMaxIndices)
    {
        int maxCount = 0;

        for (int i = 0; i < inDistanceArray.Length; i++)
        {
            if (minMaxIndices[maxCount].MaxIndex == i && i != 0)
            {
                maxCount++;
            }

            if (minMaxIndices[maxCount].MaxIndex == 0)
            {
                while (minMaxIndices[maxCount].MaxIndex == 0)
                {
                    maxCount++;
                }
            }

            if (inDistanceArray[i].InDistance)
            {
                if (GetUnitsData[maxCount].WithSquareDistance)
                {
                    UnitWithDistanceAmount unitWithDistanceAmount =
                        new UnitWithDistanceAmount(unitsMovment[i], inDistanceArray[i].SquareDistance);
                    GetUnitsData[maxCount].Unit.DistanceUnitsResults.UnitsResultAmounts[GetUnitsData[maxCount].Index]
                        .UnitsWithAmount.Add(unitWithDistanceAmount);
                }
                else
                {
                    GetUnitsData[maxCount].Unit.DistanceUnitsResults.UnitsResults[GetUnitsData[maxCount].Index].Units
                        .Add(unitsMovment[i]);
                }
            }
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
        out NativeArray<MinMaxIndex> minCountUnits,
        out NativeArray<GetAllUnitsAtDistanceData.UnitsAtDistance> unitsAtDistanceDatas)
    {
        unitsMovment = new List<UnitScript>();

        unitsAtDistanceDatas =
            new NativeArray<GetAllUnitsAtDistanceData.UnitsAtDistance>(GetUnitsData.Count, Allocator.TempJob);

        minCountUnits = new NativeArray<MinMaxIndex>(GetUnitsData.Count, Allocator.TempJob);


        for (int i = 0; i < GetUnitsData.Count; i++)
        {
            MinMaxIndex _currentMinMaxIndex = new MinMaxIndex();

            _currentMinMaxIndex.MinIndex = unitsMovment.Count;

            bool _containTypeBaseUnit =
                GetUnitsData[i].TypeMovmentUnit.Contains((int) GetUnitsData[i].Unit.SO.MovmentType);

            for (int j = 0; j < GetUnitsData[i].TypeMovmentUnit.Count; j++)
            {
                for (int k = 0; k < NeighbourCellsID[i].Count; k++)
                {
                  
                    var _allUnits = new List<UnitMovmentList>();

                    if (_containTypeBaseUnit)
                    {
                        if (GetUnitsData[i].TypeMovmentUnit[j] == (int) GetUnitsData[i].Unit.SO.MovmentType)
                        {
                            if (NeighbourCellsID[i][k] == GetUnitsData[i].Unit.Cell.ID)
                            {
                                _gridManager.Grid[NeighbourCellsID[i][k]]
                                    .AllUnits[GetUnitsData[i].Unit.MovementCellIndexList].Units
                                    .Remove(GetUnitsData[i].Unit);
                                _allUnits.AddRange(_gridManager.Grid[NeighbourCellsID[i][k]].AllUnits);
                                _gridManager.Grid[NeighbourCellsID[i][k]]
                                    .AllUnits[GetUnitsData[i].Unit.MovementCellIndexList].Units
                                    .Add(GetUnitsData[i].Unit);
                            
                            }
                            else
                            {
                                _allUnits = _gridManager.Grid[NeighbourCellsID[i][k]].AllUnits;
                                
                            }
                        }
                        else
                        {
                            _allUnits = _gridManager.Grid[NeighbourCellsID[i][k]].AllUnits;
                        }
                    }
                    else
                    {
                        _allUnits = _gridManager.Grid[NeighbourCellsID[i][k]].AllUnits;
                    }


                 
                    for (int l = 0;
                        l < _allUnits.Count;
                        l++)
                    {
                        if (GetUnitsData[i].TypeMovmentUnit[j] == (int) _allUnits[l].MovmentType)
                        {
                      
                            unitsMovment.AddRange(_allUnits[l].Units);

                            break;
                        }
                
                    }
              
                }
          
            }
          

            _currentMinMaxIndex.MaxIndex = unitsMovment.Count;
            if (_currentMinMaxIndex.MaxIndex > _currentMinMaxIndex.MinIndex)
            {
                GetAllUnitsAtDistanceData.UnitsAtDistance unitsAtDistance =
                    new GetAllUnitsAtDistanceData.UnitsAtDistance(GetUnitsData[i].DistanceCheck.Square,
                        GetUnitsData[i].Unit.transform.position, GetUnitsData[i].WithSquareDistance);
                unitsAtDistanceDatas[i] = unitsAtDistance;
                minCountUnits[i] = _currentMinMaxIndex;
            }
            else
            {
                _currentMinMaxIndex.MaxIndex = 0;
                _currentMinMaxIndex.MinIndex = 0;
            }
        }

 
    
        unitsPosition = new NativeArray<float3>(unitsMovment.Count, Allocator.TempJob);
        for (int i = 0; i < unitsMovment.Count; i++)
        {
            unitsPosition[i] = unitsMovment[i].transform.position;
        }
    }

    private void SetNeighboursCellJob(out NativeArray<GetAllUnitsAtDistanceData.NeighbourCells> neighboursCellDatas)
    {
        neighboursCellDatas =
            new NativeArray<GetAllUnitsAtDistanceData.NeighbourCells>(GetUnitsData.Count, Allocator.TempJob);

        for (int i = 0; i < GetUnitsData.Count; i++)
        {
            neighboursCellDatas[i] = new GetAllUnitsAtDistanceData.NeighbourCells(
                GetUnitsData[i].DistanceCheck.LinesCell,
                GetUnitsData[i].Unit.Cell.ID, GetUnitsData[i].Unit.Cell.LinesPosition);
        }
    }

    [BurstCompile]
    public struct GetNeighboursCellJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<GetAllUnitsAtDistanceData.NeighbourCells> AllDatas;
        [ReadOnly] public int3 GridFactors;
        [ReadOnly] public int3 GridLinesCount;


        [NativeDisableParallelForRestriction] [WriteOnly]
        public NativeArray<int> NeighbourCellsID;

        [ReadOnly] public NativeArray<int> CellIDMinCounts;


        public void Execute(int index)
        {
            var _data = AllDatas[index];

            int3 _minDistanceLines =_data.DistanceLinesCell;
            int3 _maxDistanceLines = _data.DistanceLinesCell;  


            if ((_data.LinesPosition.x + _data.DistanceLinesCell.x) > GridLinesCount.x)
                _maxDistanceLines.x = GridLinesCount.x - _data.LinesPosition.x;
            if ((_data.LinesPosition.y + _data.DistanceLinesCell.y) > GridLinesCount.y)
                _maxDistanceLines.y = GridLinesCount.y - _data.LinesPosition.y;
            if ((_data.LinesPosition.z + _data.DistanceLinesCell.z) > GridLinesCount.z)
                _maxDistanceLines.z = GridLinesCount.z - _data.LinesPosition.z;

            if ((_data.LinesPosition.x - _data.DistanceLinesCell.x) < 0)
                _minDistanceLines.x = _data.LinesPosition.x ;
            if ((_data.LinesPosition.y - _data.DistanceLinesCell.y) < 0)
                _minDistanceLines.y = _data.LinesPosition.y ;
            if ((_data.LinesPosition.z - _data.DistanceLinesCell.z) < 0)
                _minDistanceLines.z = _data.LinesPosition.z;


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
    public struct CheckUnitAtDistanceJob : IJobParallelFor
    {
        public NativeArray<CheckDistanceUnitData.UnitsAtDistance> AllDatas;

        public void Execute(int index)
        {
            var _data = AllDatas[index];
            _data.SquareDistanceCheckUnit = Vector3.SqrMagnitude(_data.CheckUnitPosition - _data.BaseUnitPosition);
            if (_data.SquareDistanceCheckUnit < _data.SquareDistance)
            {
                _data.InDistance = true;
            }

            AllDatas[index] = _data;
        }
    }

    [BurstCompile]
    public struct GetUnitsAtDistanceJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<GetAllUnitsAtDistanceData.UnitsAtDistance> AllDatas;
        [ReadOnly] public NativeArray<float3> UnitsPosition;

        [NativeDisableParallelForRestriction] public NativeArray<CheckUnitAtDistance> InDistanceArray;

        [ReadOnly] public NativeArray<MinMaxIndex> MinMaxIndexArray;

        public void Execute(int index)
        {
            var _data = AllDatas[index];
            // print(MinMaxIndexArray[index].MaxIndex);
            for (int i = MinMaxIndexArray[index].MinIndex; i < MinMaxIndexArray[index].MaxIndex; i++)
            {
                CheckUnitAtDistance checkUnit = InDistanceArray[i];
                float squareDistance = Vector3.SqrMagnitude(UnitsPosition[i] - _data.BaseUnitPosition);
                if (squareDistance <= _data.DistanceSquare)
                {
                    checkUnit.InDistance = true;
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