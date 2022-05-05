using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Mathematics;
using UnityEngine;

public class DistanceUnitsJobResults : MonoBehaviour
{
    public List<UnitListResultJobs> UnitsResults = new List<UnitListResultJobs>();
    public List<UnitResultJobsWithDistanceAmount> UnitsResultAmounts = new List<UnitResultJobsWithDistanceAmount>();
    public List<CheckUnitAtDistanceClass> CheckDistanceUnitResults = new List<CheckUnitAtDistanceClass>();


    public void AskDistanceUnit(UnitScript unitScript, DistanceUnitJob distanceCheck, List<int> movmentTypes, out int index )
    {
        GetAllUnitsAtDistanceData.DataClass _dataClass= new GetAllUnitsAtDistanceData.DataClass();
        _dataClass.SetValues(unitScript,   distanceCheck, movmentTypes, false);
        index = UnitsResults.Count;
        UnitListResultJobs unitList = new UnitListResultJobs();
        unitList.Units = new List<UnitScript>();
        UnitsResults.Add(unitList);
     DistanceUnitsJobsManager.Instance.GetUnitsData.Add(_dataClass);
    }

    public void AskDistanceUnitsWithAmount(UnitScript unitScript,DistanceUnitJob distanceCheck, List<int> movmentTypes, out int index )
    {
        GetAllUnitsAtDistanceData.DataClass _dataClass = new GetAllUnitsAtDistanceData.DataClass();
        _dataClass.SetValues(unitScript,   distanceCheck, movmentTypes, true);
        index = UnitsResultAmounts.Count;
        UnitResultJobsWithDistanceAmount unitList = new UnitResultJobsWithDistanceAmount();
        unitList.UnitsWithAmount = new List<UnitWithDistanceAmount>();
        UnitsResultAmounts.Add(unitList);
        DistanceUnitsJobsManager.Instance.GetUnitsData.Add(_dataClass);
    }

    public void AskCheckUnitAtDistance(UnitScript unitScript, DistanceUnitJob distanceCheck, UnitScript CheckUnit, out int index)
    {
        CheckDistanceUnitData.DataClass _dataClass= new CheckDistanceUnitData.DataClass();
        _dataClass.Unit = unitScript;
        _dataClass.DistanceCheck = distanceCheck;
        _dataClass.CheckUnit = CheckUnit;
        index = CheckDistanceUnitResults.Count;
        CheckUnitAtDistanceClass checkUnitAtDistanceClass = new CheckUnitAtDistanceClass();
        checkUnitAtDistanceClass.SquareDistance = -1;
        checkUnitAtDistanceClass.InDistance = false; 
        CheckDistanceUnitResults.Add(checkUnitAtDistanceClass);
        DistanceUnitsJobsManager.Instance.CheckUnitData.Add(_dataClass);
    }

    [Serializable]
    public class CheckUnitAtDistanceClass
    {
        public bool InDistance;
        public float SquareDistance;
    }
    
    
}
