using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class BoidModule : UnitModule
{
    [SerializeField] private BoidSO _boidSO;
    private List<int> _movmentTypeIndices = new List<int>();

    private int[] _indicesResults = new int[4]
    {
        -1, -1, -1,-1
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
        if(_boidSO == null)
            return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _boidSO.AllDistanceCellsClass[0].Base);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _boidSO.AllDistanceCellsClass[1].Base);
        Gizmos.color = Color.red + Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _boidSO.AllDistanceCellsClass[2].Base);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _boidSO.AllDistanceCellsClass[3].Base);
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
            for (int i = 0; i < 4; i++)
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
                
        }
       
    }


    void SendDataBoidJobs()
    {
        List<List<Transform>> _unitsTransform = new List<List<Transform>>();
        for (int i = 0; i < 3; i++)
        {
       
            List<Transform> _currentUnitsTransform = new List<Transform>();
            for (int j = 0; j < Unit.Results.UnitsResults[_indicesResults[i]].Units.Count; j++)
            {
                _currentUnitsTransform.Add(Unit.Results.UnitsResults[_indicesResults[i]].Units[j].transform);
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
        Unit.Squad.EndDestinationPoint();
    }

    void CheckDestinationUnit()
    {
        List<UnitScript> _endDestinationUnits = new List<UnitScript>();
        _endDestinationUnits.AddRange(Unit.Results.UnitsResults[_indicesResults[3]].Units);

        if (Unit.DestinationIsPoint)
        {
            if (!Unit.Squad.Destinations[Unit.DestinationIndex].FirstUnitReachedDestination)
            {
                Debug.Log(Unit.Agent.remainingDistance);
                if (Unit.Agent.remainingDistance <= _boidSO.DistanceEndDestination)
                {
                    EndDestinationPoint();
                    Unit.Squad.Destinations[Unit.DestinationIndex].FirstUnitReachedDestination = true; 
                }
            }
            else
            {
                for (int i = 0; i < _endDestinationUnits.Count; i++)
                {
                    Debug.Log(_endDestinationUnits[i].name);
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
        else if (Unit.DestinationIsUnit)
        {
        }
    }

  
}