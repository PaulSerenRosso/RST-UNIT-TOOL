using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitWithDistanceAmount
{
  public UnitScript Unit;
  public float SquareDistance;

 public  UnitWithDistanceAmount(UnitScript unit, float squareDistance)
  {
    SquareDistance = squareDistance;
    Unit = unit;
  }
}

