using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoidModule : UnitModule
{
    [SerializeField] private BoidSO _boidSO;
    private List<int> _movmentTypeIndices = new List<int>();
    private int[] _indicesResults = new int[3];

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
    public override void OnValidate()
    {
        base.OnValidate();
        if (_movmentTypeIndices.Count == 0)
            _movmentTypeIndices.Add((int) Unit.MovmentType);
    }

    public override void OnStart()
    {
        throw new System.NotImplementedException();
    }

    public override void AskUpdate()
    {
        for (int i = 0; i < 3; i++)
        {
            Unit.Results.AskDistanceUnit(Unit, _boidSO.AllDistanceCellsClass[i].DistanceJob, _movmentTypeIndices,
                out _indicesResults[i]);
        }
    }

    public override void OnUpdate()
    {
        List<List<Transform>> _unitsTransform = new List<List<Transform>>();
        for (int i = 0; i < 3; i++)
        {
            List<Transform> _currentUnitsTransform = new List<Transform>();
            for (int j = 0; j < Unit.Results.UnitsResults[_indicesResults[i]].Count; j++)
            {
                _currentUnitsTransform.Add(Unit.Results.UnitsResults[_indicesResults[i]][j].transform);
            }

            _unitsTransform.Add(_currentUnitsTransform);
        }


        List<UnitScript> _avoidanceUnits = new List<UnitScript>(Unit.Results.UnitsResults[_indicesResults[2]]);
        if (Unit.IsMove)
        {
            if (Unit.DestinationIsPoint)
            {
              
                if (Unit.Agent.pathStatus == NavMeshPathStatus.PathComplete &&
                    Unit.Agent.remainingDistance <= 0.1f)
                {
                    
                }
                else
                {
                    for (int i = 0; i < _avoidanceUnits.Count; i++)
                    {
                        
                    }
                }
            }
            else if (Unit.DestinationIsUnit)
            {
            }
        }


        UnitsBoidJobsData.UnitsBoidJobsClass _unitsBoidJobsClass = new UnitsBoidJobsData.UnitsBoidJobsClass()
        {
            Speeds = _boidSO.AllSpeeds, Unit = Unit, UnitsTransform = _unitsTransform
        };
        UnitsBoidJobsManager.Instance.BoidsData.Add(_unitsBoidJobsClass);
        for (int i = 0; i < 3; i++)
        {
            Unit.Results.UnitsResults.Remove(Unit.Results.UnitsResults[_indicesResults[i]]);
        }
    }
}