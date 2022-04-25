using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class BoidModule : UnitModule
{
    [SerializeField] private BoidSO _boidSO;
    private List<int> _movmentTypeIndices = new List<int>();

    private int[] _indicesResults = new int[3]
    {
        -1, -1, -1
    };

    // déplace le comportement de destination sur le boid module
    // faire en sorte qu'il ne puisse déclencher cette etat que quand il essaye d'atteindre une destination
    // check si l'unité à un boid module
    // il faut que je cancel la destination quand elle est atteint 

    // cas ou l'unité vise une autre unité c'est la portée d'attaque qu'il se déclenche
    // end destination

    // cas ou l'unité se déplace vers une case
    // check si l'unité doit mettre sa portée d'attaque tout le temps
    // déclenche une bool true si l'unité atteint la destination ou si elle proche du'n unité qui a atteint la destination a la range d'avoidance
    // end destination
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _boidSO.AllDistanceCellsClass[0].Base);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _boidSO.AllDistanceCellsClass[1].Base);
        Gizmos.color = Color.red + Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _boidSO.AllDistanceCellsClass[2].Base);
    }

    public override void OnValidate()
    {
        base.OnValidate();
        if (_movmentTypeIndices.Count == 0)
            _movmentTypeIndices.Add((int) Unit.MovmentType);
    }

    public override void OnStart()
    {
    }

    public override void AskUpdate()
    {
        if (Unit.IsMove)
        {
            for (int i = 0; i < 3; i++)
            {
                Unit.Results.AskDistanceUnit(Unit, _boidSO.AllDistanceCellsClass[i].DistanceJob, _movmentTypeIndices,
                    out _indicesResults[i]);
            }
        }
    }

    public override void OnUpdate()
    {
        if (Unit.IsMove)
        {
            SendDataBoidJobs();
                   CheckDestinationUnit();
                   ManageDistanceJobsResults(); 
        }
       
    }


    void SendDataBoidJobs()
    {
        List<List<Transform>> _unitsTransform = new List<List<Transform>>();
        for (int i = 0; i < 3; i++)
        {
            Debug.Log(_indicesResults[i]);
            List<Transform> _currentUnitsTransform = new List<Transform>();
            for (int j = 0; j < Unit.Results.UnitsResults[_indicesResults[i]].Count; j++)
            {
                _currentUnitsTransform.Add(Unit.Results.UnitsResults[_indicesResults[i]][j].transform);
            }

            _unitsTransform.Add(_currentUnitsTransform);
        }

        UnitsBoidJobsData.UnitsBoidJobsClass _unitsBoidJobsClass = new UnitsBoidJobsData.UnitsBoidJobsClass()
        {
            Speeds = _boidSO.AllSpeeds, Unit = Unit, UnitsTransform = _unitsTransform
        };
        UnitsBoidJobsManager.Instance.BoidsData.Add(_unitsBoidJobsClass);
    }

    void EndDestinationPoint()
    {
        Unit.DestinationIsPoint = false;
        Unit.Squad.Destinations[Unit.DestinationIndex].FirstUnitReachedDestination = false;
        Unit.IsMove = false;
    }

    void CheckDestinationUnit()
    {
        List<UnitScript> _avoidanceUnits = new List<UnitScript>(Unit.Results.UnitsResults[_indicesResults[2]]);

        if (Unit.DestinationIsPoint)
        {
            if (!Unit.Squad.Destinations[Unit.DestinationIndex].FirstUnitReachedDestination)
            {
                if (Unit.Agent.pathStatus == NavMeshPathStatus.PathComplete &&
                    Unit.Agent.remainingDistance <= 0.1f)
                {
                    EndDestinationPoint();
                }
            }
            else
            {
                for (int i = 0; i < _avoidanceUnits.Count; i++)
                {
                    if (_avoidanceUnits[i].Squad == Unit.Squad)
                    {
                        if (!_avoidanceUnits[i].DestinationIsPoint)
                        {
                            EndDestinationPoint();
                        }
                    }
                }
            }
        }
        else if (Unit.DestinationIsUnit)
        {
        }
    }

    void ManageDistanceJobsResults()
    { 
        for (int i = 0; i < 3; i++)
        {
         
          
            Unit.Results.UnitsResults.Remove(Unit.Results.UnitsResults[_indicesResults[i]]);
            _indicesResults[i] = -1;
        }
    }
}