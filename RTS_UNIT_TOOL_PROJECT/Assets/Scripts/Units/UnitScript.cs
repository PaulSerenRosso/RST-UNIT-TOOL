using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class UnitScript : MonoBehaviour
{
    public GridCell Cell;
    public Squad Squad;
    public List<UnitModule> Modules;
    public UnitMovmentType MovmentType;
    public NavMeshAgent Agent;
    public int MovementCellIndexList;
    public UnitJobsResults Results;
    public bool IsMove;
    public bool CanMove;
  public  BoidModule Boid;
  public int DestinationIndex;

    
    
    public bool
        ReachedPoint;
    public bool DestinationIsPoint;
    public bool DestinationIsUnit;
    



 
    
    private void OnValidate()
    {
        if (Results == null)
            TryGetComponent(out Results);
        
    }



    private void Start()
    {
        switch (MovmentType)
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
              
                float3 cellCount = (transform.position - GridManager.Instance.transform.position) / GridManager.Instance.SizeCells;
        
                cellCount.x = Mathf.FloorToInt(cellCount.x);
                cellCount.y = Mathf.FloorToInt(cellCount.y);
                cellCount.z = Mathf.FloorToInt(cellCount.z);
        
        
                int3 finalCellCount = (int3) cellCount;
                int idCell = (finalCellCount.x) * GridManager.Instance.CellCount.y * GridManager.Instance.CellCount.z +
                             (finalCellCount.y) * GridManager.Instance.CellCount.z + finalCellCount.z;
        
              
                Cell = GridManager.Instance.Grid[idCell];
        
                if (Cell.TryGetIndexList(MovmentType, out int index))
                {
                    if (Cell.AllUnits[index].Units == null)
                        Cell.AllUnits[index].Units = new List<UnitScript>();
        
                  Cell.AllUnits[index].Units.Add(this);
                  MovementCellIndexList = index;
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
        for (int i = 0; i < Modules.Count; i++)
        {
           Modules[i].OnStart();  
        }
      
    }

    public void AskUpdate()
    {
        Boid.AskUpdate();
        for (int i = 0; i < Modules.Count; i++)
        {
            Debug.Log(Modules.Count+"modulessssssssssssss");
            Modules[i].AskUpdate();  
        }
        if(!IsMove) return;
        for (int i = 0; i < 3; i++)
        {

            Debug.Log(Boid.Unit.Results.UnitsResults[Boid._indicesResults[i]].Units.Count);
        }
    }

    public void OnUpdate()
    {
        //  Debug.Log(index +"" + Results.UnitsResults[index].Count);

        for (int i = 0; i < 3; i++)
        {
            if(!IsMove) continue;
        
            for (int j = 0; j < Boid.Unit.Results.UnitsResults[Boid._indicesResults[i]].Units.Count ; j++)
            {
                Debug.Log(j+"   "+Boid.Unit.Results.UnitsResults[Boid._indicesResults[i]].Units.Count+ "     " +Cell.ID +"     "+ Boid.Unit.Results.UnitsResults[Boid._indicesResults[i]].Units[j].Cell.ID+"  "+ Boid.Unit.Results.UnitsResults[Boid._indicesResults[i]].Units[j].MovmentType);
            }
        }

        Boid.OnUpdate();
       
        for (int i = 0; i < Modules.Count; i++)
        {
            Modules[i].OnUpdate();  
        }
     
        Results.UnitsResults.Clear();
        
    }
}