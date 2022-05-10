using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class Squad : MonoBehaviour
{
    public List<float> SpawnAreasSize;
    public List<SpawnerUnit> SpawnerUnits;
    public PlayerName Player;
    [HideInInspector]
    public List<UnitMovmentList> AllUnits;
    [HideInInspector]
    public List<DestinationUnit> DestinationUnitList;

    [HideInInspector]
    public UnitScript TargetUnit;
    [HideInInspector]
    public Squad TargetSquad;

    [HideInInspector]
    public List<DestinationPoint> DestinationsPoint = new List<DestinationPoint>();
    [HideInInspector]
    public List<int> movmentTypeUnitsIndex;

    private float destinationUnitUpdateTimer;

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

    void UpdateDestinationUnitList()
    {

        if (!TargetUnit)
        {       TargetSquad = null; 
            TargetUnit = null;
            return; 
        }
        
        if (destinationUnitUpdateTimer <= SquadManager.Instance.DestinationUnitUpdateTime)
        {
            destinationUnitUpdateTimer += Time.deltaTime;
            return;
        }

        if (DestinationUnitList.Count != 0)
        {
            for (int i = DestinationUnitList.Count - 1; i > -1; i--)
            {
                if (DestinationUnitList[i].Units.Count == 0)
                {
                    DestinationUnitList.Remove(DestinationUnitList[i]);
                    continue;
                }

                if ((int) TargetUnit.SO.MovmentType == DestinationUnitList[i].IndexMovment)
                {
                    for (int k = 0; k < DestinationUnitList[i].Units.Count; k++)
                    {
                        DestinationUnitList[i].Position = TargetUnit.transform.position;
                        DestinationUnitList[i].Units[k].Agent
                            .SetDestination(DestinationUnitList[i].Position);
                    }
                    continue;
                }
                for (int j = 0;
                    j < MapManager.Instance.AllTerrains[(int) TargetUnit.SO.MovmentType].YDistances.Count;
                    j++)
                {
                    if (DestinationUnitList[i].IndexMovment == (int) MapManager.Instance
                        .AllTerrains[(int) TargetUnit.SO.MovmentType].YDistances[j].Type)
                    {
                        if (NavMesh.SamplePosition(TargetUnit.transform.position +
                                                   Vector3.up * MapManager.Instance
                                                       .AllTerrains[(int) TargetUnit.SO.MovmentType]
                                                       .YDistances[j].YDistance, out NavMeshHit _hit,
                            MapManager.Instance.DetectDistanceNavMesh,
                            MapManager.Instance.AllTerrains[DestinationUnitList[i].IndexMovment].NavArea))
                        {
                            DestinationUnitList[i].Position = _hit.position;
                            for (int k = 0; k < DestinationUnitList[i].Units.Count; k++)
                            {
                                DestinationUnitList[i].Units[k].Agent
                                    .SetDestination(DestinationUnitList[i].Position);
                                destinationUnitUpdateTimer = 0;
                            }
                        }
                        break;
                    }
                }
            }
        }
        else
        {
            TargetUnit = null;
            TargetSquad = null;
        }
    }

    public void OnUpdate()
    {
        UpdateDestinationUnitList();
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

        for (int i = 0; i < DestinationsPoint.Count; i++)
        {
            Gizmos.DrawWireSphere(DestinationsPoint[i].Position, 3f);
        }
    }


    public void SetUnitDestination(UnitScript unitTarget)
    {
        TargetSquad = unitTarget.Squad;
        TargetUnit = unitTarget;
        DestinationUnitList.Clear();
        DestinationsPoint.Clear();
        for (int i = 0; i < movmentTypeUnitsIndex.Count; i++)
        {
            DestinationUnit destinationUnit = new DestinationUnit(new float3(0,0,0), movmentTypeUnitsIndex[i],
                new List<UnitScript>());
            DestinationUnitList.Add(destinationUnit);
        }

        for (int i = 0; i < AllUnits.Count; i++)
        {
            for (int j = 0; j < AllUnits[i].Units.Count; j++)
            {
                AllUnits[i].Units[j].IsMove = true;
                AllUnits[i].Units[j].DestinationIsUnit = true;
                AllUnits[i].Units[j].DestinationIsPoint = false;
                for (int k = 0; k < movmentTypeUnitsIndex.Count; k++)
                {
                    if (movmentTypeUnitsIndex[i] == (int) AllUnits[i].MovmentType)
                    {
                    
                        AllUnits[i].Units[j].Agent.speed = AllUnits[i].Units[j].SO.DestinationPointSpeed;
                        AllUnits[i].Units[j].Agent.angularSpeed = AllUnits[i].Units[j].SO.DestinationPointRotationSpeed;
                        AllUnits[i].Units[j].IsEngage = false;
                        AllUnits[i].Units[j].DestinationUnitIndex = k ;
                        destinationUnitUpdateTimer = SquadManager.Instance.TargetUnitDestinationUpdateTime;
                        DestinationUnitList[k].Units.Add(AllUnits[i].Units[j]);
                        break;
                    }
                }
            }
        }
    }

    public void SetPointDestination(List<DestinationPoint> destinationSquadList)
    {
        DestinationsPoint = destinationSquadList;
        DestinationUnitList.Clear();
        for (int i = 0; i < AllUnits.Count; i++)
        {
            for (int j = 0; j < AllUnits[i].Units.Count; j++)
            {
                for (int k = 0; k < destinationSquadList.Count; k++)
                {
                    if (DestinationsPoint[k].IndexMovment == (int) AllUnits[i].MovmentType)
                    {
                        AllUnits[i].Units[j].Agent.SetDestination(destinationSquadList[k].Position);
                        AllUnits[i].Units[j].IsMove = true;
//                        Debug.Log(AllUnits[i].Units[j].name);
                        AllUnits[i].Units[j].DestinationIsPoint = true;
                        AllUnits[i].Units[j].DestinationIsUnit = false;
                        AllUnits[i].Units[j].DestinationPointIndex = k;
                        AllUnits[i].Units[j].Agent.speed = AllUnits[i].Units[j].SO.DestinationPointSpeed;
                        AllUnits[i].Units[j].Agent.angularSpeed = AllUnits[i].Units[j].SO.DestinationPointRotationSpeed;
                        AllUnits[i].Units[j].IsEngage = false;
                        break;
                    }
                }
            }
        }
    }

    public void CheckEndDestination()
    {
        
    }
    public void EndDestinationPoint()
    {
        
            for (int j = 0; j < AllUnits.Count; j++)
            {
                for (int i = 0; i < AllUnits[i].Units.Count; i++)
                {
                    AllUnits[i].Units[j].Boid.EndDestinationPoint();
                }
            }
        
    }
}