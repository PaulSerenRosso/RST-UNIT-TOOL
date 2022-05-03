using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TargetPriorityType
{
    [Serializable]
    public class Base
    {
        public float Value;

        public virtual UnitScript GetUnit(List<UnitWithDistanceAmount> Units)
        {
            return null;
        }
    }

    [Serializable]
    public class HealthPriority : Base
    {

      public   HealthPriority(HealthPriority healthPriority = null)
        {
            if(healthPriority == null)
                return;
            Value = healthPriority.Value; 
        }
        public override UnitScript GetUnit(List<UnitWithDistanceAmount> Units )
        {
            for (int i = Units.Count - 1; i > -1; i++)
            {
                if (Units[i].Unit.Health > Value)
                {
                    Units.Remove(Units[i]);
                }
            }
            Debug.Log("testouille");
            return GetCloserUnit(Units);
        }
    }

    [Serializable]
    public class DistanceMinPriority : Base
    {
        public  DistanceMinPriority(DistanceMinPriority distanceMinPriority = null)
        {
            if(distanceMinPriority == null)
                return;
            Value = distanceMinPriority.Value;
            Distance = distanceMinPriority.Distance;
        }
        public DistanceSquare Distance = new DistanceSquare();
        public override UnitScript GetUnit(List<UnitWithDistanceAmount> Units)
        {
            for (int i = Units.Count - 1; i > -1; i++)
            {
                if (Units[i].SquareDistance > Distance.Square)
                    {
                        Units.Remove(Units[i]);
                    }
            }
            return GetCloserUnit(Units);
        }
    }

    [Serializable]
    public class UnitTypePriority : Base
    {
        public int IndexEnum = 0;
        public     UnitTypePriority(UnitTypePriority unitTypePriority = null)
        {
            if(unitTypePriority == null)
                return;
            Value = unitTypePriority.Value;
         IndexEnum = unitTypePriority.IndexEnum;
        }
        public override UnitScript GetUnit(List<UnitWithDistanceAmount> Units)
        {
            for (int i = Units.Count - 1; i > -1; i++)
            {
                if (IndexEnum != Value)
                {
                    Units.Remove(Units[i]);
                }
            }
            return GetCloserUnit(Units);
        }
    }



    static UnitScript GetCloserUnit(List<UnitWithDistanceAmount> Units)
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