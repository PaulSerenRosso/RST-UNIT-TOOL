using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightModule : UnitModule
{
    [SerializeField] private int indexFightDetection;

    [SerializeField] private int indexAttackDetection;

    public UnitScript TargetUnit;
    private bool _attackIsReady;
    private float _timerAttack;
    public List<UnitsFightJobsData.UnitWithDistance> AttackUnitsList;
    bool _engagedUnitInEngageRange = false;
    bool _hasGetUnitsAttackRange = false;
    List<UnitWithDistanceAmount> _targetUnits = new List<UnitWithDistanceAmount>();
    private List<UnitWithDistanceAmount> _attackUnits = new List<UnitWithDistanceAmount>();
    private bool _engagedUnitInAttackRange;
    private int currentPriorityIndex = 0;

    private void OnDrawGizmosSelected()
    {
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
        if (Unit.DestinationIsUnit ||
            !Unit.DestinationIsPoint && !Unit.DestinationIsUnit && Unit.SO.IsEngageAutomatically)
        {
            Unit.DistanceUnitsResults.AskDistanceUnitsWithAmount(Unit, Unit.SO.DistanceEngagement.DistanceJob,
                Unit.SO.MovmentTypeIndicesDetection,
                out indexFightDetection);
        }
    }

    public void AskLateUpdate()
    {
    }

    public override void OnUpdate()
    {
        if (!_attackIsReady)
        {
            _timerAttack += Time.deltaTime;
            if (_timerAttack >= Unit.SO.CooldownAttack)
                _attackIsReady = true;
        }


        if (Unit.DestinationIsUnit ||
            !Unit.DestinationIsPoint && !Unit.DestinationIsUnit && Unit.SO.IsEngageAutomatically)
        {
            if (Unit.DistanceUnitsResults.UnitsResults[indexFightDetection].Units.Count != 0)
            {
                _engagedUnitInEngageRange = false;
                _hasGetUnitsAttackRange = false;
                _targetUnits.Clear();

                for (int i = 0;
                    i < Unit.DistanceUnitsResults.UnitsResultAmounts[indexFightDetection].UnitsWithAmount.Count;
                    i++)
                {
                    if (PlayerManager.Instance.AllPlayers[Unit.Squad.Player].EnemyPlayers.Contains(Unit
                        .DistanceUnitsResults.UnitsResultAmounts[indexFightDetection].UnitsWithAmount[i].Unit.Squad
                        .Player))
                    {
                        if (TargetUnit == Unit.DistanceUnitsResults.UnitsResultAmounts[indexFightDetection]
                            .UnitsWithAmount[i].Unit)
                            _engagedUnitInEngageRange = true;
                        _targetUnits.Add(Unit.DistanceUnitsResults.UnitsResultAmounts[indexFightDetection]
                            .UnitsWithAmount[i]);
                    }
                }

                if (!Unit.IsEngage)
                {
                    Unit.IsEngage = true;
                    Unit.Agent.speed = Unit.SO.DestinationFightSpeed;
                    Unit.Agent.angularSpeed = Unit.SO.DestinationFightRotationSpeed;
                }

                switch (Unit.SO.ChangePriority)
                {
                    case TargetChange.Automatically:
                    {
                        TargetUnit = ChangeTarget(_targetUnits);
                        break;
                    }
                    case TargetChange.DeathUnit:
                    {
                        if (!_engagedUnitInEngageRange || !TargetUnit)
                        {
                            TargetUnit = ChangeTarget(_targetUnits);
                        }
                        break;
                    }
                    case TargetChange.MaxDistanceAttack:
                    {
                        _attackUnits.Clear();
                        _engagedUnitInAttackRange = false;
                        GetUnitsAtAttackRange(out _attackUnits);
                        _hasGetUnitsAttackRange = true;
                     TargetUnit = ChangeTarget(_targetUnits);
                        break;
                    }
                }
                if(Unit.SO.WithStopMovment)
                    Debug.Log("faudra faire un truc là ");
                // mettre le stop movment

                if (_attackIsReady)
                {
                    if (!_hasGetUnitsAttackRange)
                    {
                        _engagedUnitInAttackRange = false;
                        _attackUnits.Clear();
                        GetUnitsAtAttackRange(out _attackUnits);
                    }
                    Attack();
                }
            }
        }
    }

    void GetUnitsAtAttackRange(out List<UnitWithDistanceAmount> attackUnits)
    {
      
        attackUnits = new List<UnitWithDistanceAmount>();
        for (int i = 0; i < _targetUnits.Count; i++)
        {
            if (Unit.SO.DistanceAttack.Square > _targetUnits[i].SquareDistance)
            {
                attackUnits.Add(_targetUnits[i]);
                if (TargetUnit == _targetUnits[i].Unit)
                {
                    _engagedUnitInAttackRange = true;
                }
            }
        }


    }

    void Attack()
    {
        if (Unit.SO.ChangePriority == TargetChange.MaxDistanceAttack)
        {
            if (!_engagedUnitInAttackRange)
            {
                Attack();
            }
        }
    }

    UnitScript ChangeTarget(List<UnitWithDistanceAmount> units)
    {
        for (int i = 0; i < currentPriorityIndex + 1; i++)
        {
            if (CheckPriority(Unit.SO.TargetPriorities[i], units, out UnitScript _newUnitTarget))
            {
                return _newUnitTarget;
            }
        }
        return GetCloserUnit(units);
    }

   bool CheckPriorityCondition(TargetPriorityClass targetPriorityClass,UnitWithDistanceAmount unitWithDistanceAmount )
    {
        switch (targetPriorityClass.Enum)
        {
            case TargetPriority.LessHealth:
            {
                if ((int) unitWithDistanceAmount.Unit.Health <= targetPriorityClass.Value)
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
                if ((int) unitWithDistanceAmount.Unit.SO.Type== targetPriorityClass.Value)
                    return true;
                return false;
            }
        }

        return false;
    }
    public bool CheckPriority(TargetPriorityClass targetPriorityClass, List<UnitWithDistanceAmount> units,  out UnitScript targetUnit)
    {
        for (int i = units.Count - 1; i > -1; i++)
        {
            if (!CheckPriorityCondition(targetPriorityClass, units[i] ))
            {
                    units.Remove(units[i]);
            }
        }
        if (units.Count != 0)
        {
            targetUnit = GetCloserUnit(units);
            return true;
        }
        targetUnit = null;
        return false;
    }
    
    UnitScript GetCloserUnit(List<UnitWithDistanceAmount> Units)
    {
        int index = 0;
        for (int i = 0; i < Units.Count; i++)
        {
            if (Units[index].SquareDistance >= Units[i].SquareDistance)
            {
                index = i;
            }
        }
        return Units[index].Unit;
    }
}