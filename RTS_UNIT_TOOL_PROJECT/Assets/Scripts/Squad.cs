using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public List<UnitScript> AllUnits;
   public List<SpawnerUnit> SpawnerUnits;
    public bool IsSelected;
    public Squad Target;
    public float Destination;
    public PlayerName Player;


    public void OnStart()
    {
        
        for (int i = 0; i < AllUnits.Count; i++)
        {
            AllUnits[i].OnStart();
        }   
    }

    public void OnUpdate()
    {
        for (int i = 0; i < AllUnits.Count; i++)
        {
            AllUnits[i].OnUpdate();
        }   
    }
    
    
}
