using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class BoidModule : UnitModule
{
    public int[] _indicesResults = new int[4]
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
        if(Unit.SO == null)
            return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, Unit.SO.AllDistanceCellsClass[0].Base);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, Unit.SO.AllDistanceCellsClass[1].Base);
        Gizmos.color = Color.red + Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Unit.SO.AllDistanceCellsClass[2].Base);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Unit.SO.AllDistanceCellsClass[3].Base);
    }



    public override void OnStart()
    {
    }

    public override void AskUpdate()
    {
        if (Unit.IsMove && !Unit.IsEngage)
        {
            for (int i = 0; i < 3; i++)
            {
                Unit.DistanceUnitsResults.AskDistanceUnit(Unit, Unit.SO.AllDistanceCellsClass[i].DistanceJob, Unit.SO.MovmentTypeList,
                    out _indicesResults[i]);
//                Debug.Log(_indicesResults[i]);
           
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
       
///            Debug.Log(Unit.Results.UnitsResults[_indicesResults[i]].Units.Count);
            List<Transform> _currentUnitsTransform = new List<Transform>();
            for (int j = 0; j < Unit.DistanceUnitsResults.UnitsResults[_indicesResults[i]].Units.Count; j++)
            {
             //Debug.Log(i+" cellid"+Unit.Cell.ID +"     "+ Unit.Results.UnitsResults[_indicesResults[i]].Units[j].Cell.ID);

//          Debug.Log( i+ ""+Unit.Cell.ID+"     "+ Unit.Results.UnitsResults[_indicesResults[i]].Units[j].Cell.ID+"     "+  Vector3.Distance(Unit.Results.UnitsResults[_indicesResults[i]].Units[j].transform.position, transform.position));
               if(Unit.DistanceUnitsResults.UnitsResults[_indicesResults[i]].Units[j].Squad == Unit.Squad && !Unit.DistanceUnitsResults.UnitsResults[_indicesResults[i]].Units[j].IsDead)
                _currentUnitsTransform.Add(Unit.DistanceUnitsResults.UnitsResults[_indicesResults[i]].Units[j].transform);
            }

            _unitsTransform.Add(_currentUnitsTransform);
        }

        UnitsBoidJobsData.UnitsBoidJobsClass _unitsBoidJobsClass = new UnitsBoidJobsData.UnitsBoidJobsClass()
        {
            Speeds = Unit.SO.AllSpeeds, Unit = Unit, UnitsTransform = _unitsTransform
        };
        UnitsBoidJobsManager.Instance.BoidsData.Add(_unitsBoidJobsClass);
    }

    void EndDestinationPoint()
    {
        Unit.DestinationIsPoint = false;
        Unit.Squad.Destinations[Unit.DestinationIndex].FirstUnitReachedDestination = true;
        Unit.IsMove = false;
        Unit.Agent.SetDestination(transform.position);
        Unit.Squad.EndDestinationPoint();
    }

    void CheckDestinationUnit()
    {
        List<UnitScript> _endDestinationUnits = new List<UnitScript>();
        _endDestinationUnits.AddRange(Unit.DistanceUnitsResults.UnitsResults[_indicesResults[2]].Units);

        if (Unit.DestinationIsPoint)
        {
            if (!Unit.Squad.Destinations[Unit.DestinationIndex].FirstUnitReachedDestination)
            {
            
                if (!Unit.Agent.pathPending && Unit.Agent.remainingDistance <= Unit.SO.AllDistanceCellsClass[2].Base)
                {
                    EndDestinationPoint();
                    Unit.Squad.Destinations[Unit.DestinationIndex].FirstUnitReachedDestination = true; 
                }
            }
            else
            {
               Debug.Log(_endDestinationUnits.Count); 
                for (int i = 0; i < _endDestinationUnits.Count; i++)
                {
          
                    if (_endDestinationUnits[i].Squad == Unit.Squad)
                    {
                        if (!_endDestinationUnits[i].DestinationIsPoint)
                        {
                            Debug.Log(Unit.Cell.ID+"bonjour");
                            EndDestinationPoint();
                        }
                    }
                }
            }
        }
    
    }

  
}