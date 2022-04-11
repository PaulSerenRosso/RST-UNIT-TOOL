using UnityEngine;
using UnityEngine.AI;

public class UnitScript : MonoBehaviour
{
    public GridCell Cell;
    public Squad Squad;
    public UnitModuleManager ModuleManager;
    public UnitMovmentType MovmentType;
    public NavMeshAgent Agent;


    
    


    public void OnStart()
    {
        Agent.SetDestination(Squad.transform.position);
        ModuleManager.OnStart();
    }
    
   public void OnUpdate()
    {
        ModuleManager.OnUpdate();
    }
}
