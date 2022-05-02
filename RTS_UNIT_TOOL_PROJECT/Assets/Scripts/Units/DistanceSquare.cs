using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
[Serializable]
public class DistanceSquare: MonoBehaviour
{
   public float Base;
   public float Square;

   public void BaseToSquare()
   {
       Square = Base * Base;
   }
}
