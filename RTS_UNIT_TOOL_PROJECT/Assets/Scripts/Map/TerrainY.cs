using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

[Serializable]
public class TerrainY
{
    public Color Color;
    public UnitMovmentType MovmentType;
    public float YMaxPosition;
    public float YPosition;
  public Terrain Terrain;
  public LayerMask LayerMask;
  public float SizeYTerrain;
  public int NavArea;
  public List<TerrainYDistance> YDistances;

}
