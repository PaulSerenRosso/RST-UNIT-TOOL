using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class GridManager : MonoBehaviour
{

public List<GridCell> _grid = new List<GridCell>();
public int3 CellCount;
  private float3 _sizeCells;
   [SerializeField] private float3 _sizeGrid;
   public int3 CellFactor;
   
   private void OnValidate()
   {
      if(_sizeGrid.Equals(float3.zero))
         return;
         if(CellCount.Equals(int3.zero))
            return;
         CellFactor = new int3(CellCount.y * CellCount.z, CellCount.z, 1);
      _sizeCells = new float3(_sizeGrid.x/CellCount.x, _sizeGrid.y / CellCount.y, _sizeGrid.z/CellCount.z);
      _grid.Clear();
     
      for (int x = 0; x < CellCount.x; x++)
      {
         for (int y = 0; y < CellCount.y; y++)
         {
            for (int z = 0; z < CellCount.z; z++)
            {
               GridCell cell = new GridCell();
               cell.MinPosition =
                (float3) transform.position + new float3(x * _sizeCells.x, y * _sizeCells.y, _sizeCells.z*z);
               cell.MaxPosition = cell.MinPosition + _sizeCells;
               cell.CenterPosition = cell.MinPosition + _sizeCells / 2;
               _grid.Add(cell);
               cell.id = _grid.Count - 1;
            }
         }
      }
   }

   private void OnDrawGizmosSelected()
   {
      Gizmos.color = Color.green;
     
         Gizmos.DrawWireCube((float3) transform.position + _sizeGrid / 2,_sizeGrid );
      Gizmos.color = Color.red;
      for (int i = 0; i < _grid.Count; i++)
      { ;
         Gizmos.DrawWireCube(_grid[i].CenterPosition,_sizeCells );
      }
   }


   
}
