using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class UnitScript : MonoBehaviour
{
    public UnitSO SO;
    [HideInInspector]
    public GridCell Cell;
    [HideInInspector]
    public Squad Squad;
    [HideInInspector]
    public float HealthPercentage;
    
 public FightModule Fight;
 public NavMeshAgent Agent;
 public DistanceUnitsJobResults DistanceUnitsResults;
    public BoidModule Boid;
 [HideInInspector]
    public int MovementCellIndexList;
[HideInInspector]
    public bool IsMove;
    [HideInInspector]
    public int DestinationPointIndex;
    [HideInInspector]
    public int DestinationUnitIndex;

    [HideInInspector]
    public bool IsDead;
    [HideInInspector]
    public float Health;
    [HideInInspector]
    public bool IsEngage;
    [HideInInspector]
    public bool
        ReachedPoint;
    [HideInInspector]
    public bool DestinationIsPoint;
    
    [HideInInspector]
    public bool DestinationIsUnit;


    private void OnValidate()
    {
        if (DistanceUnitsResults == null)
            TryGetComponent(out DistanceUnitsResults);
    }


    private void Start()
    {
        Health = SO.MaxHealth;
        HealthPercentage = 100;
        switch (SO.MovmentType)
        {
            case UnitMovmentType.Aerial:
            {
                Agent.areaMask = MapManager.Instance.AllTerrains[0].NavArea;
                break;
            }
            case UnitMovmentType.Earthly:
            {
                Agent.areaMask = MapManager.Instance.AllTerrains[1].NavArea;
                break;
            }
            case UnitMovmentType.Underground:
            {
                Agent.areaMask = MapManager.Instance.AllTerrains[2].NavArea;
                break;
                
            }
        }
    }

    IEnumerator UpdateCellAfterAgentPosition()
    {
        yield return new WaitForEndOfFrame();
        GridManager.Instance.Grid[Cell.ID].AllUnits[MovementCellIndexList].Units.Remove(this);

        float3 cellCount = (transform.position - GridManager.Instance.transform.position) /
                           GridManager.Instance.SizeCells;

        cellCount.x = Mathf.FloorToInt(cellCount.x);
        cellCount.y = Mathf.FloorToInt(cellCount.y);
        cellCount.z = Mathf.FloorToInt(cellCount.z);


        int3 finalCellCount = (int3) cellCount;
        int idCell = (finalCellCount.x) * GridManager.Instance.CellCount.y * GridManager.Instance.CellCount.z +
                     (finalCellCount.y) * GridManager.Instance.CellCount.z + finalCellCount.z;


        Cell = GridManager.Instance.Grid[idCell];

        if (Cell.TryGetIndexList(SO.MovmentType, out int index))
        {
            if (Cell.AllUnits[index].Units == null)
                Cell.AllUnits[index].Units = new List<UnitScript>();

            Cell.AllUnits[index].Units.Add(this);
            MovementCellIndexList = index;
            GridManager.Instance.Grid[idCell] = Cell;
        }
        else
        {
            Debug.LogError("the movment type cell isn't valid");
        }
    }

    public void OnStart()
    {
        StartCoroutine(UpdateCellAfterAgentPosition());
        Boid.OnStart();
        Fight.OnStart();
    }

    public void AskUpdate()
    {
        Boid.AskUpdate();
    
        Fight.AskUpdate();

        if (!IsMove) return;
    
    }

    public void OnUpdate()
    {
        //  Debug.Log(index +"" + Results.UnitsResults[index].Count);

        Boid.OnUpdate();

        Fight.OnUpdate();


        DistanceUnitsResults.UnitsResultAmounts.Clear();
        DistanceUnitsResults.UnitsResults.Clear();
        DistanceUnitsResults.CheckDistanceUnitResults.Clear();
    }

    public void TakeDamage(float amount)
    {
        if (IsDead)
            return;
        Health -= amount;
        HealthPercentage = Health / SO.MaxHealth * 100;
        if (Health <= 0)
        {
            IsDead = true;
            SquadManager.Instance.DeadUnits.Add(this);
        }
    }
}