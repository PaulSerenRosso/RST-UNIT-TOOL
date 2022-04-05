using System;
using Unity.Mathematics;

[Serializable]
public class GridCell
{
    public int id;
     public float3 MinPosition;
    public float3 MaxPosition;
    public float3 CenterPosition;
}
