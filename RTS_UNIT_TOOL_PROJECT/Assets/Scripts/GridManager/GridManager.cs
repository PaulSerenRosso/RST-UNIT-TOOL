using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class GridManager : MonoBehaviour
{

public List<GridCell> Grid = new List<GridCell>();
public int3 CellCount;

public float3 SizeCells;
  [SerializeField] public float3 SizeGrid;
   public int3 CellFactor;
   
   public static GridManager Instance;
   void Awake()
   {
      if (Instance != null && Instance != this)
         Destroy(gameObject);    // Suppression d'une instance précédente (sécurité...sécurité...)
 
      Instance = this;
   }
   private void OnDrawGizmosSelected()
   {
      Gizmos.color = Color.green;
     
         Gizmos.DrawWireCube((float3) transform.position + SizeGrid / 2,SizeGrid );
      for (int i = 0; i < Grid.Count; i++)
      { ;
      Gizmos.color = Grid[i].Color;
   
         Gizmos.DrawWireCube(Grid[i].CenterPosition,SizeCells );
      }
      
  
   
   }

   private void OnDrawGizmos()
   {
      for (int i = 0; i < Grid.Count; i++)
      { ;
         Gizmos.color = Grid[i].Color;
   
         Gizmos.DrawWireCube(Grid[i].CenterPosition,SizeCells/5);
      }
   }
}
