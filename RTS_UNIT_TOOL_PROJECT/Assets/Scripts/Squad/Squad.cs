using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public List<float> SpawnAreasSize;
    public List<UnitMovmentList> AllUnits;
    public List<SpawnerUnit> SpawnerUnits;


    //sert de bool pour savoir si on attaque ou pas
    public UnitScript TargetUnit;
    public Squad TargetSquad;


    public PlayerName Player;

    public List<DestinationSquad> Destinations = new List<DestinationSquad>();

    public List<int> movmentTypeUnitsIndex;


    private void Start()
    {
        if (!SquadManager.Instance.AllSquads.Contains(this))
        {
            SquadManager.Instance.AllSquads.Add(this);
            for (int i = 0; i < AllUnits.Count; i++)
            {
                SquadManager.Instance.AllUnits.AddRange(AllUnits[i].Units);
            }
        }
    }

    public void OnStart()
    {
        for (int i = 0; i < AllUnits.Count; i++)
        {
            for (int j = 0; j < AllUnits[i].Units.Count; j++)
            {
                AllUnits[i].Units[j].OnStart();
            }
        }
    }

    public void OnUpdate()
    {
        
        for (int i = 0; i < AllUnits.Count; i++)
        {
            for (int j = 0; j < AllUnits[i].Units.Count; j++)
            {
                AllUnits[i].Units[j].OnUpdate();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        for (int i = 0; i < Destinations.Count; i++)
        {
            Gizmos.DrawWireSphere(Destinations[i].Position, 3f);
        }
    }

    public void SetUnitDestination(List<DestinationSquad> destinationSquadList, UnitScript unitTarget)
    {
        Destinations = destinationSquadList;
        TargetSquad = unitTarget.Squad;
        TargetUnit = unitTarget;


        for (int i = 0; i < AllUnits.Count; i++)
        {
            for (int j = 0; j < AllUnits[i].Units.Count; j++)
            {
                if(!AllUnits[i].Units[j].CanMove)
                    continue;
                AllUnits[i].Units[j].IsMove = true;
                AllUnits[i].Units[j].DestinationIsUnit = true;
                for (int k = 0; k < destinationSquadList.Count; k++)
                {
                    AllUnits[i].Units[j].DestinationIndex = k; 
                }
            }
        }
    }

    public void SetPointDestination(List<DestinationSquad> destinationSquadList)
    {
        Destinations = destinationSquadList;
        for (int i = 0; i < AllUnits.Count; i++)
        {
            for (int j = 0; j < AllUnits[i].Units.Count; j++)
            {
                if(!AllUnits[i].Units[j].CanMove)
                    continue;
                for (int k = 0; k < destinationSquadList.Count; k++)
                {
                    if (destinationSquadList[k].IndexMovment == (int) AllUnits[i].MovmentType)
                    {
                        AllUnits[i].Units[j].Agent.SetDestination(destinationSquadList[k].Position);
                        AllUnits[i].Units[j].IsMove = true;
                        AllUnits[i].Units[j].DestinationIsPoint = true;
                        AllUnits[i].Units[j].DestinationIndex = k; 
                        break;
                    }
                }
            }
        }
    }
}

