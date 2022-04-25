using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MinMaxIndex
{
    public int MaxIndex;
    public int MinIndex;

  public  MinMaxIndex(MinMaxIndexClass minMaxIndexClass)
    {
        MaxIndex = minMaxIndexClass.MaxIndex;
        MinIndex = minMaxIndexClass.MinIndex;
    }
}

public class MinMaxIndexClass
{
    public int MaxIndex;
    public int MinIndex;
}
