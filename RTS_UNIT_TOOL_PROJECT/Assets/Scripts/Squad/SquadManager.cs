
using System.Collections;
using System.Collections.Generic;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine;


public class SquadManager : MonoBehaviour
{
    [HideInInspector]
    public List<UnitScript> DeadUnits;
    [HideInInspector]
    public List<Squad> AllSquads;
 
    public List<string> AllTypesUnit;
    [HideInInspector]
    public List<UnitScript> AllUnits = new List<UnitScript>();
    private List<UnitScript> _currentUnitsMove = new List<UnitScript>();
   
    [Header("Optismisation Timer")]
    public float DestinationUnitUpdateTime;
    public float TargetUnitDestinationUpdateTime;
    [SerializeField] private int _updateCellJobsSize;
    [SerializeField] private GridManager _gridManager;
    public static SquadManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject); // Suppression d'une instance précédente (sécurité...sécurité...)

        Instance = this;
    }

    void Start()
    {
        StartCoroutine(WaitForAddSquads());
    }

   IEnumerator WaitForAddSquads()
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < AllSquads.Count; i++)
        {
            AllSquads[i].OnStart();
        }
    }

    void DestroyDeadUnit()
    {
        int j = 0;
        for (int i = DeadUnits.Count - 1; i > -1; i--, j++)
        {
            DeadUnits[i].Cell.AllUnits[DeadUnits[i].MovementCellIndexList].Units.Remove(DeadUnits[i]);
            _gridManager.Grid[DeadUnits[i].Cell.ID].AllUnits[DeadUnits[i].MovementCellIndexList].Units
                .Remove(DeadUnits[i]);
            for (int k = 0; k < DeadUnits[i].Squad.AllUnits.Count; k++)
            {
                if (DeadUnits[i].Squad.AllUnits[k].MovmentType == DeadUnits[i].SO.MovmentType)
                {
                    DeadUnits[i].Squad.AllUnits[k].Units.Remove(DeadUnits[i]);
                    if (DeadUnits[i].Squad.AllUnits[k].Units.Count == 0)
                        DeadUnits[i].Squad.AllUnits.Remove(DeadUnits[i].Squad.AllUnits[k]);
                    if (DeadUnits[i].Squad.AllUnits.Count == 0)
                    {
                        AllSquads.Remove(DeadUnits[i].Squad);
                        PlayerManager.Instance.AllPlayers[DeadUnits[i].Squad.Player].AllSquads
                            .Remove(DeadUnits[i].Squad);
                        Destroy(DeadUnits[i].Squad.gameObject);
                    }

                    break;
                }
            }

            AllUnits.Remove(DeadUnits[i]);

            Destroy(DeadUnits[i].gameObject);
            DeadUnits.RemoveAt(i);
        }
    }

    private void Update()
    {
        DestroyDeadUnit();

        UpdateUnitCell();
      
             
        for (int i = 0; i < AllUnits.Count; i++)
        {
            AllUnits[i].AskUpdate();
        }
        
        DistanceUnitsJobsManager.Instance.CheckDistanceUnit();
        
        for (int i = 0; i < AllUnits.Count; i++)
        {
            AllUnits[i].Fight.AskLateUpdate();
        }
        
             DistanceUnitsJobsManager.Instance.GetAllUnitsAtDistance();
          
        

       


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
            if (AllUnits[i].IsMove)
                _currentUnitsMove.Add(AllUnits[i]);
        }

        if (_currentUnitsMove.Count == 0)
            return;
        var _updateUnitCellData = new NativeArray<UpdateUnitCellData>(_currentUnitsMove.Count, Allocator.TempJob);
        for (int i = 0; i < _currentUnitsMove.Count; i++)
        {
            _updateUnitCellData[i] =
                new UpdateUnitCellData( _currentUnitsMove[i], GridManager.Instance);
        }

        UpdateUnitCellJob _updateUnitCell = new UpdateUnitCellJob
        {
            AllData = _updateUnitCellData
        };
        JobHandle _handle = _updateUnitCell.Schedule(_currentUnitsMove.Count, _updateCellJobsSize);
        _handle.Complete();
        SetNewUnitCells(_updateUnitCell);
        _updateUnitCellData.Dispose();
    }

    void SetNewUnitCells(UpdateUnitCellJob updateUnitCellJob)
    {
        for (int i = 0; i < _currentUnitsMove.Count; i++)
        {
            if (updateUnitCellJob.AllData[i].CurrentId == -1)
                continue;
            if (_currentUnitsMove[i].Cell.ID == updateUnitCellJob.AllData[i].CurrentId)
                continue;
            if (_currentUnitsMove[i].Cell.TryGetIndexList(_currentUnitsMove[i].SO.MovmentType, out int RemoveIndex))
            {
                _currentUnitsMove[i].Cell.AllUnits[RemoveIndex].Units.Remove(_currentUnitsMove[i]);
//                 Debug.Log("test");
            }

            _currentUnitsMove[i].Cell = GridManager.Instance.Grid[updateUnitCellJob.AllData[i].CurrentId];
            if (_currentUnitsMove[i].Cell.TryGetIndexList(_currentUnitsMove[i].SO.MovmentType, out int AddIndex))
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
        public int3 LinesPosition;
        public int3 GridLineCount;

        public UpdateUnitCellData( UnitScript unit, GridManager gridManager)
        {
            UnitPosition = unit.transform.position;
            MinCellPosition = unit.Cell.MinPosition;
            MaxCellPosition = unit.Cell.MaxPosition;
            CurrentId = unit.Cell.ID;
            OffSet = new int3(0, 0, 0);
            CellFactor = gridManager.CellFactor;
            GridLineCount = gridManager.CellCount;
            LinesPosition = unit.Cell.LinesPosition;
        }
    }

    [BurstCompile]
    public struct UpdateUnitCellJob : IJobParallelFor
    {
        public NativeArray<UpdateUnitCellData> AllData;

        public void Execute(int index)
        {
            var data = AllData[index];

            if (data.UnitPosition.x > data.MaxCellPosition.x)
            {
                if(data.LinesPosition.x != data.GridLineCount.x)
                data.OffSet.x += 1;
            }

            else if (data.UnitPosition.x < data.MinCellPosition.x)
            {
                if(data.LinesPosition.x != 0)
                  data.OffSet.x -= 1;
            }
              

            if (data.UnitPosition.y > data.MaxCellPosition.y)
            {
                if (data.LinesPosition.y != data.GridLineCount.y)
                {
                    data.OffSet.y += 1;
                }
               
            }
                
            else if (data.UnitPosition.y < data.MinCellPosition.y)
            {
                if(data.LinesPosition.y != 0)
                data.OffSet.y -= 1;
            }
                

            if (data.UnitPosition.z > data.MaxCellPosition.z)
            { 
                if(data.LinesPosition.z != data.GridLineCount.z)
                  data.OffSet.z += 1;
            }
              
            else if (data.UnitPosition.z < data.MinCellPosition.z)
            {
                if(data.LinesPosition.z != 0)
                 data.OffSet.z -= 1;
            }
               

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