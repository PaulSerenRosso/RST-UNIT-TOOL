using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    public List<Squad> AllSquads;
    public List<string> AllTypesUnit;
    private List<UnitScript> _allUnits;
    
    


   
   void Start()
   {
       for (int i = 0; i < AllSquads.Count; i++)
       {
           AllSquads[i].OnStart();
       }
   }
   

    private void OnValidate()
    {
        _allUnits.Clear();
        for (int i = 0; i <AllSquads.Count ; i++)
        {
          _allUnits.AddRange(AllSquads[i].AllUnits);  
        }
    }

    private void Update()
    {
        // update la destruction d'unité sur le maintread
     UpdateUnitCell();
     for (int i = 0; i < AllSquads.Count; i++)
        {
            AllSquads[i].OnUpdate();
        }
    }

    void UpdateUnitCell()
    {
        
         var _updateUnitCellData = new NativeArray<UpdateUnitCellData>(_allUnits.Count, Allocator.TempJob);
         for (int i = 0; i < _allUnits.Count; i++)
                {
                    _updateUnitCellData[i] = new UpdateUnitCellData(_allUnits[i].Cell,_allUnits[i], _gridManager);
                }
                UpdateUnitCellJob updateUnitCell = new UpdateUnitCellJob
                {
                  AllData = _updateUnitCellData
                };
                JobHandle _handle=updateUnitCell.Schedule(_allUnits.Count, 50);
                _handle.Complete();
                
                for (int i = 0; i < _allUnits.Count; i++)
                {
                    _allUnits[i].Cell = _gridManager._grid[_updateUnitCellData[i].CurrentId];
                }
                _updateUnitCellData.Dispose();
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
            CurrentId = gridCell.id;
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

            if(data.OffSet.Equals(int3.zero))
                return;
            data.OffSet *= data.CellFactor;
            data.CurrentId += data.OffSet.x + data.OffSet.y + data.OffSet.z;
            AllData[index] = data;
        }
    }
}
