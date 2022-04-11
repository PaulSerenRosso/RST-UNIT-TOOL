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
      
       //NavMesh.GetAreaFromName(MovmentType.ToString());
    }


    public void OnStart()
    {
        Agent.enabled = false;
        Debug.Log("test");
        Agent.Warp(new Vector3(transform.position.x,MapManager.Instance.AllTerrains[(int) MovmentType].Terrain.transform.position.y,transform.position.z));
        Agent.enabled = true;
        ModuleManager.OnStart();
    }
    
   public void OnUpdate()
    {
        ModuleManager.OnUpdate();
    }
}
