using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UnitJobsResults : MonoBehaviour
{
    public List<UnitListResultJobs> UnitsResults = new List<UnitListResultJobs>();
 
    public void AskDistanceUnit(UnitScript unitScript, DistanceUnitJob distanceCheck, List<int> movmentTypes, out int index)
    {
        UnitDistanceJobData.UnitsDistanceClass _unitsDistanceClass = new UnitDistanceJobData.UnitsDistanceClass();
        _unitsDistanceClass.SetValues(unitScript,  distanceCheck, movmentTypes);
        index = UnitsResults.Count;
        UnitListResultJobs unitList = new UnitListResultJobs();
        unitList.Units = new List<UnitScript>();
        UnitsResults.Add(unitList);
      UnitsDistanceJobsManager.Instance.DistanceUnitsData.Add(_unitsDistanceClass);
    }
    
}
