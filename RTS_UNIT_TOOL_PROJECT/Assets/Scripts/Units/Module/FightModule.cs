using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class FightModule : UnitModule
{
    private int indexFightDetection;

    [HideInInspector]
    public UnitWithDistanceAmount TargetUnit = new UnitWithDistanceAmount();
    private bool _attackIsReady;
    private float _timerAttack;

    List<UnitWithDistanceAmount> _targetUnits = new List<UnitWithDistanceAmount>();


    private int currentPriorityIndex = 0;
    private bool _isNewTarget;
    private bool _inFight;
    private float3 _targetPosition;



    private float timerUpdateTargetUnitDestination;

    private void OnDrawGizmosSelected()
    {
        if(Unit.SO == null) return;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, Unit.SO.DistanceEngagement.Base);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Unit.SO.DistanceAttack.Base);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, Unit.SO.DistanceStopMovment.Base);
    }

    public override void OnValidate()
    {
        base.OnValidate();
    }

    public override void OnStart()
    {
    }

    public override void AskUpdate()
    {
        if (Unit.IsEngage || Unit.DestinationIsUnit ||( !Unit.IsEngage &&
            !Unit.DestinationIsPoint && !Unit.DestinationIsUnit && Unit.SO.IsEngageAutomatically))
        {
            _inFight = true;
            _isNewTarget = false;

            if (TargetUnit.Unit)
            {
             
                if (!CheckPriorityChange())
                {
             
                    Unit.DistanceUnitsResults.AskCheckUnitAtDistance(Unit, Unit.SO.DistanceEngagement.DistanceJob,
                        TargetUnit.Unit, out indexFightDetection);
                }
                else
                {
               
                    TargetUnit.Unit = null; 
                    Unit.DistanceUnitsResults.AskDistanceUnitsWithAmount(Unit, Unit.SO.DistanceEngagement.DistanceJob,
                        Unit.SO.MovmentTypeIndicesDetection,
                        out indexFightDetection);
                    _isNewTarget = true;
                }
            }
            else
            {
            
                TargetUnit.Unit = null; 
                Unit.DistanceUnitsResults.AskDistanceUnitsWithAmount(Unit, Unit.SO.DistanceEngagement.DistanceJob,
                    Unit.SO.MovmentTypeIndicesDetection,
                    out indexFightDetection);

                _isNewTarget = true;
            }
        }
        else if(_inFight)
        {
            _inFight = false;
        }
    }


    public void AskLateUpdate()
    {
        if (_inFight)
        {
            if (TargetUnit.Unit)
            {
                if (!_isNewTarget)
                {
                    try
                    { 
                       
                        if (!Unit.DistanceUnitsResults.CheckDistanceUnitResults[indexFightDetection].InDistance)
                                         {
                                             TargetUnit.Unit = null;
                                             Unit.DistanceUnitsResults.AskDistanceUnitsWithAmount(Unit,
                                                 Unit.SO.DistanceEngagement.DistanceJob,
                                                 Unit.SO.MovmentTypeIndicesDetection,
                                                 out indexFightDetection);
                                             _isNewTarget = true;
                                         }
                        else
                        {
//                            Debug.Log("essaye de lire ça");
                            TargetUnit.SquareDistance =
                                Unit.DistanceUnitsResults.CheckDistanceUnitResults[indexFightDetection].SquareDistance;
                        }
                    }
                    catch (Exception e)
                    {
                        Unit.DistanceUnitsResults.AskDistanceUnitsWithAmount(Unit,
                            Unit.SO.DistanceEngagement.DistanceJob,
                            Unit.SO.MovmentTypeIndicesDetection,
                            out indexFightDetection);
                        _isNewTarget = true;
                    }
                }
            }
        }
    }


    public override void OnUpdate()
    {
        if (!_attackIsReady)
        {
            _timerAttack += Time.deltaTime;
            if (_timerAttack >= Unit.SO.CooldownAttack)
                _attackIsReady = true;
        }

        if (_inFight)
        {
            _targetUnits.Clear();
            if (_isNewTarget)
            {
                FightNewTarget();
            }
            else
            {
                Fight();
            }
        }
    }

    void UpdateAgentToTargetUnit()
    {
   
        if (timerUpdateTargetUnitDestination < SquadManager.Instance.TargetUnitDestinationUpdateTime)
        {
            timerUpdateTargetUnitDestination += Time.deltaTime;
            return;
        }

        timerUpdateTargetUnitDestination = 0;

        if (TargetUnit.Unit.SO.MovmentType == Unit.SO.MovmentType)
        {
            _targetPosition = TargetUnit.Unit.transform.position;
            Unit.Agent.SetDestination(_targetPosition);
        }
        else
        {
            for (int j = 0;
                j < MapManager.Instance.AllTerrains[(int) TargetUnit.Unit.SO.MovmentType].YDistances.Count;
                j++)
            {
                if (Unit.SO.MovmentType == MapManager.Instance
                    .AllTerrains[(int) TargetUnit.Unit.SO.MovmentType].YDistances[j].Type)
                {
                    if (NavMesh.SamplePosition(TargetUnit.Unit.transform.position +
                                               Vector3.up * MapManager.Instance
                                                   .AllTerrains[(int) TargetUnit.Unit.SO.MovmentType]
                                                   .YDistances[j].YDistance, out NavMeshHit _hit,
                        MapManager.Instance.DetectDistanceNavMesh,
                        MapManager.Instance.AllTerrains[(int) Unit.SO.MovmentType].NavArea))

                        _targetPosition = _hit.position;
                    Unit.Agent.SetDestination(_targetPosition);
                    break;
                }
            }
        }
    }

    void GetEnemiesUnits()
    {
        for (int i = 0;
            i < Unit.DistanceUnitsResults.UnitsResultAmounts[indexFightDetection].UnitsWithAmount.Count;
            i++)
        {
         //   Debug.Log(Unit.Cell.ID + "    " + Unit
           //     .DistanceUnitsResults.UnitsResultAmounts[indexFightDetection].UnitsWithAmount[i].Unit.Squad
             //   .Player);
            if (PlayerManager.Instance.AllPlayers[Unit.Squad.Player].EnemyPlayers.Contains(Unit
                .DistanceUnitsResults.UnitsResultAmounts[indexFightDetection].UnitsWithAmount[i].Unit.Squad
                .Player))
            {
                _targetUnits.Add(Unit.DistanceUnitsResults.UnitsResultAmounts[indexFightDetection]
                    .UnitsWithAmount[i]);
            }
        }
    }

    void FightNewTarget()
    {
     
        if (Unit.DistanceUnitsResults.UnitsResultAmounts[indexFightDetection].UnitsWithAmount.Count != 0)
        {
            GetEnemiesUnits();

            if (_targetUnits.Count != 0)
            {


             if (Unit.DestinationIsUnit)
             {
                 Unit.DestinationIsUnit = false;
                 Unit.Squad.DestinationUnitList[Unit.DestinationUnitIndex].Units.Remove(Unit);
             }
      
                TargetUnit = ChangeTarget(_targetUnits);
                if (!Unit.IsEngage)
                {
                    Unit.IsEngage = true;
                    Unit.Agent.speed = Unit.SO.DestinationFightSpeed;
                    Unit.Agent.angularSpeed = Unit.SO.DestinationFightRotationSpeed;
                    Unit.IsMove = true;
                }

               
                CheckStopMovmentDistance();
                Attack();
                UpdateAgentToTargetUnit();
            }
            else
            {
              
                EndFight();
            }
        }
        else
        {
            EndFight();
        }
    }

    public bool CheckEndFight()
    {
        if (!Unit.DestinationIsUnit || Unit.SO.IsEngageAutomatically) return true;
        return false;
    }

    public void EndFight()
    {
        bool targetNull = Unit.DestinationIsUnit && !Unit.Squad.TargetUnit;
        if (targetNull)
        {
            Unit.DestinationIsPoint = false; 
        }
        if (CheckEndFight() || targetNull)
        {
            Unit.Agent.SetDestination(transform.position);
            Unit.IsMove = false;
            Unit.Agent.speed = 0;
            Unit.IsEngage = false;
            Unit.Agent.angularSpeed = 0;
        }
    }

    void CheckStopMovmentDistance()
    {
        if (TargetUnit.SquareDistance >= Unit.SO.DistanceStopMovment.Square &&
            Unit.Agent.speed != Unit.SO.DestinationFightSpeed)
        {
            Unit.Agent.speed = Unit.SO.DestinationFightSpeed;
            Unit.Agent.angularSpeed = Unit.SO.DestinationFightRotationSpeed;
        }
        else if (Unit.SO.WithStopMovment && TargetUnit.SquareDistance <= Unit.SO.DistanceStopMovment.Square)
        {
            Unit.Agent.speed = 0;
            Unit.Agent.angularSpeed = 0;
        }
    }

    void Fight()
    {
      //  Debug.Log("bonsoir à toi le ifht");
        if (!Unit.IsEngage)
        {
            Unit.IsEngage = true;
            Unit.Agent.speed = Unit.SO.DestinationFightSpeed;
            Unit.Agent.angularSpeed = Unit.SO.DestinationFightRotationSpeed;
        }

        CheckStopMovmentDistance();
        Attack();
        UpdateAgentToTargetUnit();
    }

    bool CheckPriorityChange()
    {
        switch (Unit.SO.ChangePriority)
        {
            case TargetChange.Automatically:
            {
                return true;
            }
            case TargetChange.MaxDistanceAttack:
            {
                if (TargetUnit.SquareDistance <= Unit.SO.DistanceAttack.Square)
                {
                    return false;
                }

                return true;
            }
        }

        return false;
    }


    void Attack()
    {
 
        if (_attackIsReady)
        {
          
            if (TargetUnit.SquareDistance <= Unit.SO.DistanceAttack.Square)
            {
             //   Debug.Log("tesfdsdsqfdsqfdsqfdsqftaaaa");
                if (Unit.SO.IsRandomDamage)
                {
                    
                    TargetUnit.Unit.TakeDamage( Random.Range(Unit.SO.MinDamage, Unit.SO.MaxDamage));
                }
                else
                {
                    TargetUnit.Unit.TakeDamage(Unit.SO.BaseDamage);
                }

                _attackIsReady = false;
                _timerAttack = 0;
            }
        }
    }



    UnitWithDistanceAmount ChangeTarget(List<UnitWithDistanceAmount> units)
    {
        for (int i = 0; i < Unit.SO.TargetPriorities.Count; i++)
        {
            if (CheckPriority(Unit.SO.TargetPriorities[i], units, out UnitWithDistanceAmount _newUnitTarget))
            {
                return _newUnitTarget;
            }
        }
      //  Debug.Log(units.Count);
        return GetCloserUnit(units);
    }

    bool CheckPriorityCondition(TargetPriorityClass targetPriorityClass, UnitWithDistanceAmount unitWithDistanceAmount)
    {
        switch (targetPriorityClass.Enum)
        {
            case TargetPriority.LessHealth:
            {
                if ((int) unitWithDistanceAmount.Unit.HealthPercentage <= targetPriorityClass.Value)
                    return true;
                return false;
            }
            case TargetPriority.SquareDistanceMin:
            {
                if (unitWithDistanceAmount.SquareDistance > targetPriorityClass.Value)
                    return true;
                return false;
            }
            case TargetPriority.UnitType:
            {
                if ((int) unitWithDistanceAmount.Unit.SO.Type == targetPriorityClass.Value)
                    return true;
                return false;
            }
        }

        return false;
    }

    public bool CheckPriority(TargetPriorityClass targetPriorityClass, List<UnitWithDistanceAmount> units,
        out UnitWithDistanceAmount targetUnit)
    {
        List<UnitWithDistanceAmount> _units = new List<UnitWithDistanceAmount>(units); 
        for (int i = _units.Count - 1; i > -1; i--)
        {
            if (!CheckPriorityCondition(targetPriorityClass, _units[i]))
            {
                _units.Remove(_units[i]);
            }
        }

        if (_units.Count != 0)
        {
            targetUnit = GetCloserUnit(_units);
            return true;
        }

        targetUnit = null;
        return false;
    }

    UnitWithDistanceAmount GetCloserUnit(List<UnitWithDistanceAmount> Units)
    {
        int index = 0;
        for (int i = 0; i < Units.Count; i++)
        {
            if (Units[index].SquareDistance <= Units[i].SquareDistance)
            {
                index = i;
            }
        }
        return Units[index];
    }
}