using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class UnitWithDistanceAmount
{
  public UnitScript Unit;
  public float SquareDistance;

 public  UnitWithDistanceAmount(UnitScript unit, float squareDistance)
  {
    SquareDistance = squareDistance;
    Unit = unit;
  }

 public UnitWithDistanceAmount()
 {
  
 }
}

