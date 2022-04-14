using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class GridManager : MonoBehaviour
{

public List<GridCell> Grid = new List<GridCell>();
public int3 CellCount;
 [HideInInspector]
public float3 SizeCells;
  [SerializeField] public float3 SizeGrid;
   public int3 CellFactor;
   
  
   private void OnDrawGizmosSelected()
   {
      Gizmos.color = Color.green;
     
         Gizmos.DrawWireCube((float3) transform.position + SizeGrid / 2,SizeGrid );
      for (int i = 0; i < Grid.Count; i++)
      { ;
      Gizmos.color = Grid[i].Color;
   
         Gizmos.DrawWireCube(Grid[i].CenterPosition,SizeCells );
         
      }
      Gizmos.DrawSphere(Grid[79].CenterPosition, 1f);
   
   }


   
}
