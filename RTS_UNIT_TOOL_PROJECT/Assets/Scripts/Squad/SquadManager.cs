using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    public List<Squad> AllSquads;
    public List<string> AllTypesUnit;
    public List<UnitScript> AllUnits = new List<UnitScript>();
    private List<UnitScript> _currentUnitsMove = new List<UnitScript>();


    [SerializeField]
    private int _updateCellJobsSize;
    public static SquadManager Instance { get; private set; }
 
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);    // Suppression d'une instance précédente (sécurité...sécurité...)
 
        Instance = this;
    }
   void Start()
   {
       for (int i = 0; i < AllSquads.Count; i++)
       {
           AllSquads[i].OnStart();
       }
       
   }
   

    private void OnValidate()
    {
        AllUnits.Clear();
        for (int i = 0; i <AllSquads.Count ; i++)
        {
            for (int j = 0; j < AllSquads[i].AllUnits.Count; j++)
            {
          AllUnits.AddRange(  AllSquads[i].AllUnits[j].Units);  
              
            }
        }
    }

    private void Update()
    { 
        Debug.Log("IL FAUT QUE CHECK " +_gridManager.Grid[1322].AllUnits[0].Units.Count);
        // update la destruction d'unité sur le maintread
     UpdateUnitCell();
     for (int i = 0; i < AllUnits.Count; i++)
     {
        AllUnits[i].AskUpdate();
     }
    UnitsDistanceJobsManager.Instance.OnUpdate();    
     for (int i = 0; i < AllSquads.Count; i++)
     {
         AllSquads[i].OnUpdate();
     }
     UnitsBoidJobsManager.Instance.OnUpdate();
    }

    void UpdateUnitCell()
    {
        _currentUnitsMove.Clear();
         for (int i = 0; i < AllUnits.Count; i++)
         {
             if(AllUnits[i].IsMove && AllUnits[i].CanMove)
                 _currentUnitsMove.Add(AllUnits[i]);
         }
         if(_currentUnitsMove.Count == 0)
             return;
         var _updateUnitCellData = new NativeArray<UpdateUnitCellData>(_currentUnitsMove.Count, Allocator.TempJob);
         for (int i = 0; i < _currentUnitsMove.Count; i++)
                {
                    _updateUnitCellData[i] = new UpdateUnitCellData(_currentUnitsMove[i].Cell,_currentUnitsMove[i], _gridManager);
                }
                UpdateUnitCellJob _updateUnitCell = new UpdateUnitCellJob
                {
                  AllData = _updateUnitCellData
                };
                JobHandle _handle=_updateUnitCell.Schedule(_currentUnitsMove.Count, _updateCellJobsSize);
                _handle.Complete();
                SetNewUnitCells(_updateUnitCell);
                _updateUnitCellData.Dispose();
        
    }

    void SetNewUnitCells(UpdateUnitCellJob updateUnitCellJob)
    {
        for (int i = 0; i < _currentUnitsMove.Count; i++)
        {
            if(updateUnitCellJob.AllData[i].CurrentId == -1)
                continue;
            if(_currentUnitsMove[i].Cell.ID == updateUnitCellJob.AllData[i].CurrentId )
                continue;
            if (_currentUnitsMove[i].Cell.TryGetIndexList(_currentUnitsMove[i].MovmentType, out int RemoveIndex))
            {
                 _currentUnitsMove[i].Cell.AllUnits[RemoveIndex].Units.Remove(_currentUnitsMove[i]);
                 Debug.Log("test");
            }
               
            _currentUnitsMove[i].Cell = _gridManager.Grid[updateUnitCellJob.AllData[i].CurrentId];
            if (_currentUnitsMove[i].Cell.TryGetIndexList(_currentUnitsMove[i].MovmentType, out int AddIndex))
                _currentUnitsMove[i].Cell.AllUnits[AddIndex].Units.Add(_currentUnitsMove[i]);
            _currentUnitsMove[i].MovementCellIndexList = AddIndex;
        }
    }

    public struct UpdateUnitCellData
    {
        public float3 UnitPosition;
        public float3 MinCellPosition;
        public float3 MaxCellPosition;
        public int CurrentId;
        public int3 OffSet;
        public int3 CellFactor;
        public UpdateUnitCellData(GridCell gridCell, UnitScript unit, GridManager gridManager)
        {
            UnitPosition = unit.transform.position;
            MinCellPosition = gridCell.MinPosition;
            MaxCellPosition = gridCell.MaxPosition;
            CurrentId = gridCell.ID;
            OffSet = new int3(0,0,0);
            CellFactor = gridManager.CellFactor;
        }
    }
    
    [BurstCompile]
    public struct  UpdateUnitCellJob : IJobParallelFor
    {
        public NativeArray<UpdateUnitCellData> AllData;

        public void Execute(int index)
        {
            var data = AllData[index];
            
            if (data.UnitPosition.x > data.MaxCellPosition.x)
                data.OffSet.x += 1;
           else if (data.UnitPosition.x < data.MinCellPosition.x)
                data.OffSet.x -= 1;
            
            if (data.UnitPosition.y > data.MaxCellPosition.y)
                data.OffSet.y += 1;
            else if (data.UnitPosition.y < data.MinCellPosition.y)
                data.OffSet.y -= 1;
            
            if (data.UnitPosition.z > data.MaxCellPosition.z)
                data.OffSet.z += 1;
            else if (data.UnitPosition.z < data.MinCellPosition.z)
                data.OffSet.z -= 1;

            if (data.OffSet.Equals(int3.zero))
            {
                data.CurrentId = -1; 
                return;
            }
            data.OffSet *= data.CellFactor;
            data.CurrentId += data.OffSet.x + data.OffSet.y + data.OffSet.z;
            AllData[index] = data;
        }
    }
}
