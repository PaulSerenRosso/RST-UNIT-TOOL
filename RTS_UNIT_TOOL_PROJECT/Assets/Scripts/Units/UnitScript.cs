using System;
using System.Collections.Generic;
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
        
        Modules.Clear();
        UnitModule[] modules = GetComponents<UnitModule>();
        Modules.AddRange(modules);
        if (GetComponent<BoidModule>())
        {
           CanMove = true; 
        }
        else
        {
            CanMove = false;
        }
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

    public void OnStart()
    {
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
            Modules[i].AskUpdate();  
        }
    }

    public void OnUpdate()
    {
        //  Debug.Log(index +"" + Results.UnitsResults[index].Count);

        Boid.OnUpdate();
        for (int i = 0; i < Modules.Count; i++)
        {
            Modules[i].OnUpdate();  
        }
        Results.UnitsResults.Clear();
        
    }
}