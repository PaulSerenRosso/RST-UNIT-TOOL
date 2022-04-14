using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitScript : MonoBehaviour
{
    public GridCell Cell;
    public Squad Squad;
    public UnitModuleManager ModuleManager;
    public UnitMovmentType MovmentType;
    public bool isMove; 
    public NavMeshAgent Agent;
    public int MovementCellIndexList;


    private void OnValidate()
    {
        switch (MovmentType)
       {
           case UnitMovmentType.Aerial:
           { 
               Agent.areaMask = 8;
               break;
               
           }
           case UnitMovmentType.Earthly:
           {
               Agent.areaMask = 32;
               break;
               
           }
           case UnitMovmentType.Underground:
           { 
               Agent.areaMask = 16;
               break;
               
           }
       }
    }
    public void OnStart()
    {
        ModuleManager.OnStart();
    }
    
   public void OnUpdate()
    {
        ModuleManager.OnUpdate();
    }
}
