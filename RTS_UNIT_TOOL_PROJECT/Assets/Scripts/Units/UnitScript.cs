using System;
using UnityEngine;
using UnityEngine.AI;

public class UnitScript : MonoBehaviour
{
    public GridCell Cell;
    public Squad Squad;
    public UnitModuleManager ModuleManager;
    public UnitMovmentType MovmentType;
    public NavMeshAgent Agent;


    private void OnValidate()
    {
       Debug.Log(NavMesh.GetAreaFromName(MovmentType.ToString()));
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
