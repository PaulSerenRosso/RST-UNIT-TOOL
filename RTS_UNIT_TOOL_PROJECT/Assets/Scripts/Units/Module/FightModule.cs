using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;

public class FightModule : UnitModule
{
    [SerializeField] private int indexFightDetection;

    [SerializeField] private int indexAttackDetection;

    public UnitWithDistanceAmount TargetUnit;
    private bool _attackIsReady;
    private float _timerAttack;
    public List<UnitsFightJobsData.UnitWithDistance> AttackUnitsList;
    bool _engagedUnitInEngageRange = false;
    bool _hasGetUnitsAttackRange = false;
    List<UnitWithDistanceAmount> _targetUnits = new List<UnitWithDistanceAmount>();
    private List<UnitWithDistanceAmount> _attackUnits = new List<UnitWithDistanceAmount>();
    private bool _engagedUnitInAttackRange;
    private int currentPriorityIndex = 0;
    private bool _isNewTarget;
    private bool _inFight;

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
        _inFight = false;
        if (Unit.DestinationIsUnit ||
            !Unit.DestinationIsPoint && !Unit.DestinationIsUnit && Unit.SO.IsEngageAutomatically)
        {
            _inFight = true;
            _isNewTarget = false;
            if (TargetUnit.Unit)
            {
                Unit.DistanceUnitsResults.AskCheckUnitAtDistance(Unit, Unit.SO.DistanceEngagement.DistanceJob,
                    TargetUnit.Unit, out indexFightDetection);
            }
            else
            {
                Unit.DistanceUnitsResults.AskDistanceUnitsWithAmount(Unit, Unit.SO.DistanceEngagement.DistanceJob,
                    Unit.SO.MovmentTypeIndicesDetection,
                    out indexFightDetection);
                _isNewTarget = true;
            }
        }
    }

    public void AskLateUpdate()
    {
        if (_inFight)
        {
            if (TargetUnit.Unit)
            {
                if (Unit.DistanceUnitsResults.CheckDistanceUnitResults[indexFightDetection].InDistance)
                {
                    Unit.DistanceUnitsResults.AskDistanceUnitsWithAmount(Unit, Unit.SO.DistanceEngagement.DistanceJob,
                        Unit.SO.MovmentTypeIndicesDetection,
                        out indexFightDetection);
                    _isNewTarget = true;
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
            _hasGetUnitsAttackRange = false;
            _targetUnits.Clear();
            if (_isNewTarget)
            {
                FightNewTarget();
            }
            else
            {
                _engagedUnitInEngageRange = true;
                Fight();
            }
        }
    }

    void FightNewTarget()
    {
        if (Unit.DistanceUnitsResults.UnitsResults[indexFightDetection].Units.Count != 0)
        {
            for (int i = 0;
                i < Unit.DistanceUnitsResults.UnitsResultAmounts[indexFightDetection].UnitsWithAmount.Count;
                i++)
            {
                if (PlayerManager.Instance.AllPlayers[Unit.Squad.Player].EnemyPlayers.Contains(Unit
                    .DistanceUnitsResults.UnitsResultAmounts[indexFightDetection].UnitsWithAmount[i].Unit.Squad
                    .Player))
                {
                    _targetUnits.Add(Unit.DistanceUnitsResults.UnitsResultAmounts[indexFightDetection]
                        .UnitsWithAmount[i]);
                }
            }

            CheckPriorityChange();
            if (!Unit.IsEngage)
            {
                Unit.IsEngage = true;
                Unit.Agent.speed = Unit.SO.DestinationFightSpeed;
                Unit.Agent.angularSpeed = Unit.SO.DestinationFightRotationSpeed;
            }

            CheckStopMovmentDistance();
            if (_attackIsReady)
            {
                if (!_hasGetUnitsAttackRange)
                {
                    _engagedUnitInAttackRange = false;
                    _attackUnits.Clear();
                    GetUnitsInAttackRange(out _attackUnits);
                }

                Attack();
            }
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
        CheckPriorityChange();
        if (!Unit.IsEngage)
        {
            Unit.IsEngage = true;
            Unit.Agent.speed = Unit.SO.DestinationFightSpeed;
            Unit.Agent.angularSpeed = Unit.SO.DestinationFightRotationSpeed;
        }

        CheckStopMovmentDistance();


        if (_attackIsReady)
        {
            if (!_hasGetUnitsAttackRange)
            {
                _engagedUnitInAttackRange = false;
                _attackUnits.Clear();
                GetUnitsInAttackRange(out _attackUnits);
            }

            Attack();
        }
    }

    void CheckPriorityChange()
    {
        switch (Unit.SO.ChangePriority)
        {
            case TargetChange.Automatically:
            {
                TargetUnit = ChangeTarget(_targetUnits);
                break;
            }
            case TargetChange.DeathUnit:
            {
                if (!_isNewTarget || !TargetUnit.Unit)
                {
                    TargetUnit = ChangeTarget(_targetUnits);
                }

                break;
            }
            case TargetChange.MaxDistanceAttack:
            {
                if (!_isNewTarget || TargetUnit.SquareDistance <= Unit.SO.DistanceAttack.Square)
                {
                    _attackUnits.Clear();
                    _engagedUnitInAttackRange = false;
                    GetUnitsInAttackRange(out _attackUnits);
                    _hasGetUnitsAttackRange = true;
                    TargetUnit = ChangeTarget(_attackUnits);
                }

                break;
            }
        }
    }

    void GetUnitsInAttackRange(out List<UnitWithDistanceAmount> attackUnits)
    {
        attackUnits = new List<UnitWithDistanceAmount>();
        for (int i = 0; i < _targetUnits.Count; i++)
        {
            if (Unit.SO.DistanceAttack.Square > _targetUnits[i].SquareDistance)
            {
                attackUnits.Add(_targetUnits[i]);
                _engagedUnitInAttackRange = true;
            }
        }
    }

    void Attack()
    {
        if (!_engagedUnitInAttackRange)
        {
            if (Unit.SO.IsRandomDamage)
            {
                JobHandle _handle = new JobHandle();
                RandomDamageJob _randomDamageJob = new RandomDamageJob()
                {
                    Damage = 0, MaxDamage = Unit.SO.MaxDamage, MinDamage = Unit.SO.MinDamage
                };
                _randomDamageJob.Schedule();
                _handle.Complete();
                TargetUnit.Unit.TakeDamage(_randomDamageJob.Damage, Unit);
            }
            else
            {
                TargetUnit.Unit.TakeDamage(Unit.SO.BaseDamage, Unit);
            }

            _timerAttack = 0;
        }
    }

    [BurstCompile]
    public struct RandomDamageJob : IJob
    {
        public float Damage;
        public float MinDamage;
        public float MaxDamage;

        public void Execute()
        {
            Damage = Random.Range(MinDamage, MaxDamage);
        }
    }


    UnitWithDistanceAmount ChangeTarget(List<UnitWithDistanceAmount> units)
    {
        for (int i = 0; i < currentPriorityIndex + 1; i++)
        {
            if (CheckPriority(Unit.SO.TargetPriorities[i], units, out UnitWithDistanceAmount _newUnitTarget))
            {
                return _newUnitTarget;
            }
        }

        return GetCloserUnit(units);
    }

    bool CheckPriorityCondition(TargetPriorityClass targetPriorityClass, UnitWithDistanceAmount unitWithDistanceAmount)
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
        for (int i = units.Count - 1; i > -1; i++)
        {
            if (!CheckPriorityCondition(targetPriorityClass, units[i]))
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

    UnitWithDistanceAmount GetCloserUnit(List<UnitWithDistanceAmount> Units)
    {
        int index = 0;
        for (int i = 0; i < Units.Count; i++)
        {
            if (Units[index].SquareDistance >= Units[i].SquareDistance)
            {
                index = i;
            }
        }

        return Units[index];
    }
}