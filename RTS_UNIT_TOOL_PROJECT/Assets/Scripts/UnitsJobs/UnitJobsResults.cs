using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UnitJobsResults : MonoBehaviour
{
    public List<List<UnitScript>> UnitsResults = new List<List<UnitScript>>();

    public void AskDistanceUnit(UnitScript unitScript, DistanceUnitJob distanceCheck, List<int> movmentTypes, out int index)
    {
        UnitDistanceJobData.UnitsDistanceClass _unitsDistanceClass = new UnitDistanceJobData.UnitsDistanceClass();
        _unitsDistanceClass.SetValues(unitScript,  distanceCheck, movmentTypes);
        index = UnitsResults.Count;
        UnitsResults.Add(new List<UnitScript>());
      UnitsDistanceJobsManager.Instance.DistanceUnitsData.Add(_unitsDistanceClass);
    }
    
}
