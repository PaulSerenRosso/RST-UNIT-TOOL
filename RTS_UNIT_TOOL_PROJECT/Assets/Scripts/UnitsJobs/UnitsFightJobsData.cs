using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsFightJobsData 
{

   public class UnitDistanceAmountClass
   {
      public List<UnitScript> Units;
      public FightModule Fight;
      public List<UnitWithDistance> Results;
   }
   
   public class UnitWithDistance
   {
      public UnitScript Unit;
      public float Distance;
   }
}
