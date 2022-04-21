using System;
using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public List<float> SpawnAreasSize;
    public List<UnitScript> AllUnits;
   public List<SpawnerUnit> SpawnerUnits;
    public bool IsSelected;
    public Squad Target;
    public float Destination;
    public PlayerName Player;


  

    private void Start()
    {
        if (!SquadManager.Instance.AllSquads.Contains(this))
        {
            SquadManager.Instance.AllSquads.Add(this);
            SquadManager.Instance.AllUnits.AddRange(AllUnits);
        }
    }

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
