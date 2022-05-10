using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoidModule : UnitModule
{
    private int indexCohesion = -1;
    private List<List<UnitScript>> _unitsBoid = new List<List<UnitScript>>();

    [HideInInspector]
    public Vector3 OldVelocity;
    void OnDrawGizmosSelected()
    {
        if (Unit.SO == null)
            return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, Unit.SO.AllDistanceCellsClass[0].Base);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, Unit.SO.AllDistanceCellsClass[1].Base);
        Gizmos.color = Color.red + Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Unit.SO.AllDistanceCellsClass[2].Base);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Unit.SO.DistanceToDestinationPoint);
    }


    public override void OnStart()
    {
    }

    public override void AskUpdate()
    {
        _unitsBoid.Clear();
        if (Unit.IsMove && !Unit.IsEngage)
        {
            Unit.DistanceUnitsResults.AskDistanceUnitsWithAmount(Unit, Unit.SO.AllDistanceCellsClass[0].DistanceJob,
                Unit.SO.MovmentTypeList,
                out indexCohesion);
        }
    }

    public override void OnUpdate()
    {
        if (Unit.IsMove)
        {
            if (!Unit.IsEngage)
            {
                SendDataBoidJobs();

                if (!Unit.DestinationIsUnit)
                {
                    CheckDestinationPoint();
                }
            }
        }
    }


    void SendDataBoidJobs()
    {
        _unitsBoid = new List<List<UnitScript>>()
        {
            new List<UnitScript>(), new List<UnitScript>(), new List<UnitScript>()
        };
        for (int j = 0; j < Unit.DistanceUnitsResults.UnitsResultAmounts[indexCohesion].UnitsWithAmount.Count; j++)
        {
            if (Unit.DistanceUnitsResults.UnitsResultAmounts[indexCohesion].UnitsWithAmount[j].Unit.Squad ==
                Unit.Squad &&
                !Unit.DistanceUnitsResults.UnitsResultAmounts[indexCohesion].UnitsWithAmount[j].Unit.IsDead)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (Unit.DistanceUnitsResults.UnitsResultAmounts[indexCohesion].UnitsWithAmount[j].SquareDistance <=
                        Unit.SO.AllDistanceCellsClass[i].DistanceJob.Square)
                    {
                        _unitsBoid[i].Add(Unit.DistanceUnitsResults.UnitsResultAmounts[indexCohesion].UnitsWithAmount[j]
                            .Unit);
                    }
                }
            }
        }
        UnitsBoidJobsData.UnitsBoidJobsClass _unitsBoidJobsClass = new UnitsBoidJobsData.UnitsBoidJobsClass()
        {
            Speeds = Unit.SO.AllSpeeds, Unit = Unit, Units = _unitsBoid, OldVelocity = OldVelocity,
            SmoothFactor = Unit.SO.BoidSmoothFactor
        };
        UnitsBoidJobsManager.Instance.BoidsData.Add(_unitsBoidJobsClass);
    }

    public void EndDestinationPoint()
    {
        Unit.Agent.speed = 0;
        Unit.Agent.angularSpeed = 0;
        Unit.DestinationIsPoint = false;
        Unit.Squad.DestinationsPoint[Unit.DestinationPointIndex].FirstUnitReachedDestination = true;

        Unit.IsMove = false;
        Unit.Agent.SetDestination(transform.position);
    }

    void CheckDestinationPoint()
    {
        if (Unit.DestinationIsPoint)
        {
            List<UnitScript> _endDestinationUnits = new List<UnitScript>();
            //Debug.Log("coehsion"+Unit.DistanceUnitsResults.UnitsResults[_indicesResults[0]].Units.Count);
            //  Debug.Log("alignement"+Unit.DistanceUnitsResults.UnitsResults[_indicesResults[1]].Units.Count);
            //   Debug.Log("avoidance"+Unit.DistanceUnitsResults.UnitsResults[_indicesResults[2]].Units.Count);
       
            _endDestinationUnits.AddRange(_unitsBoid[2]);
            
            if (!Unit.Squad.DestinationsPoint[Unit.DestinationPointIndex].FirstUnitReachedDestination)
            {
                if (!Unit.Agent.pathPending && Unit.Agent.remainingDistance <= Unit.SO.DistanceToDestinationPoint)
                {
                    EndDestinationPoint();
                    Unit.Squad.DestinationsPoint[Unit.DestinationPointIndex].FirstUnitReachedDestination = true;
                }
            }
            else
            {
                for (int i = 0; i < _endDestinationUnits.Count; i++)
                {
                    if (_endDestinationUnits[i].Squad == Unit.Squad)
                    {
                        if (!_endDestinationUnits[i].DestinationIsPoint)
                        {
                            EndDestinationPoint();
                        }
                    }
                }
            }
        }
    }
}