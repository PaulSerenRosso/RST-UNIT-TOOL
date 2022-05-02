using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightModule : UnitModule
{
    [SerializeField] private int indexFightDetection;

    [SerializeField] private int indexAttackDetection;

  private bool _attackIsReady;
    private float _timerAttack;
    public List<UnitsFightJobsData.UnitWithDistance> AttackUnitsList;
    
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
        if (Unit.DestinationIsUnit || !Unit.DestinationIsPoint && !Unit.DestinationIsUnit && Unit.SO.IsEngageAutomatically)
        {
            Unit.DistanceUnitsResults.AskDistanceUnitsWithAmount(Unit, Unit.SO.DistanceEngagement.DistanceJob, Unit.SO.MovmentTypeIndicesDetection ,
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
             if (_timerAttack >= Unit.SO.DelayShoot)
                 _attackIsReady = true;
        }
        
           
        if (Unit.DestinationIsUnit || !Unit.DestinationIsPoint && !Unit.DestinationIsUnit && Unit.SO.IsEngageAutomatically)
        {
            if (Unit.DistanceUnitsResults.UnitsResults[indexFightDetection].Units.Count != 0)
            {
                List<UnitWithDistanceAmount> _targetUnits = new List<UnitWithDistanceAmount>();
                for (int i = 0; i < Unit.DistanceUnitsResults.UnitsResultAmounts[indexFightDetection].UnitsWithAmount.Count ; i++)
                {
                    if(PlayerManager.Instance.AllPlayers[Unit.Squad.Player].EnemyPlayers.Contains(Unit.DistanceUnitsResults.UnitsResultAmounts[indexFightDetection].UnitsWithAmount[i].Unit.Squad.Player))
                    {
                        _targetUnits.Add(Unit.DistanceUnitsResults.UnitsResultAmounts[indexFightDetection].UnitsWithAmount[i]);
                    }
                }
                if (!Unit.IsEngage)
                { 
                    Unit.IsEngage = true;
                    Unit.Agent.speed = Unit.SO.DestinationFightSpeed;
                    Unit.Agent.angularSpeed = Unit.SO.DestinationFightRotationSpeed;
                }

                if (Unit.SO.Change == TargetChange.MaxDistanceAttack || _attackIsReady)
                { 
                    List<UnitWithDistanceAmount> _attackUnits = new List<UnitWithDistanceAmount>();
                    for (int i = 0; i < _targetUnits.Count; i++) { if (Unit.SO.DistanceAttack.Square > _targetUnits[i].SquareDistance) { _attackUnits.Add( _targetUnits[i]); }
                    }
                }
            }
        }
        
    }
}
