using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class UnitScript : MonoBehaviour
{
    public GridCell Cell;
    public Squad Squad;
    public UnitModuleManager ModuleManager;
    public UnitMovmentType MovmentType;
    public bool isMove;
    public NavMeshAgent Agent;
    public int MovementCellIndexList;
    public UnitJobsResults Results;

    public DistanceCellsClass testDistance;
  public  List<int> test = new List<int>();
  public int index;
  [SerializeField]
  private bool testst; 
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

        if (Results == null) 
            TryGetComponent<UnitJobsResults>(out Results);

    
        testDistance.ConvertDistanceToDistanceCell(FindObjectOfType<GridManager>());
    }
    public void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position,testDistance.Base);
        }
    public void OnStart()
    {
        ModuleManager.OnStart();
    }

    public void AskUpdate()
    {
        if (!testst)
        {
            Results.AskDistanceUnit(this,testDistance.DistanceJob,test, out int newIndex);
            index = newIndex;
            testst = true;
        }
           
        ModuleManager.AskUpdate();
    }
    public void OnUpdate()
    {
        Debug.Log(index +"" + Results.UnitsResults[index].Count);
    
        ModuleManager.OnUpdate();
        
       
            for (int i = 0; i <Results.UnitsResults[index].Count ; i++)
            {
                Debug.Log("base"+Cell.ID);
                Debug.Log(Results.UnitsResults[index][i].Cell.ID);
            }
            testst = false;
              Results.UnitsResults.Clear();
        
        
      
    }
}