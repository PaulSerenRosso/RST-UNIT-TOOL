using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DistanceUnitsJobResults : MonoBehaviour
{
    public List<UnitListResultJobs> UnitsResults = new List<UnitListResultJobs>();
    public List<UnitResultJobsWithDistanceAmount> UnitsResultAmounts = new List<UnitResultJobsWithDistanceAmount>();

    public void AskDistanceUnit(UnitScript unitScript, DistanceUnitJob distanceCheck, List<int> movmentTypes, out int index )
    {
        DistanceUnitsJobData.UnitsDistanceClass _unitsDistanceClass = new DistanceUnitsJobData.UnitsDistanceClass();
        _unitsDistanceClass.SetValues(unitScript,   distanceCheck, movmentTypes, false);
        index = UnitsResults.Count;
        UnitListResultJobs unitList = new UnitListResultJobs();
        unitList.Units = new List<UnitScript>();
        UnitsResults.Add(unitList);
     DistanceUnitsJobsManager.Instance.DistanceUnitsData.Add(_unitsDistanceClass);
    }

    public void AskDistanceUnitsWithAmount(UnitScript unitScript, DistanceUnitJob distanceCheck, List<int> movmentTypes, out int index )
    {
        DistanceUnitsJobData.UnitsDistanceClass _unitsDistanceClass = new DistanceUnitsJobData.UnitsDistanceClass();
        _unitsDistanceClass.SetValues(unitScript,   distanceCheck, movmentTypes, true);
        index = UnitsResultAmounts.Count;
        UnitResultJobsWithDistanceAmount unitList = new UnitResultJobsWithDistanceAmount();
        unitList.UnitsWithAmount = new List<UnitWithDistanceAmount>();
        UnitsResultAmounts.Add(unitList);
        DistanceUnitsJobsManager.Instance.DistanceUnitsData.Add(_unitsDistanceClass);
    }
    
    
}
