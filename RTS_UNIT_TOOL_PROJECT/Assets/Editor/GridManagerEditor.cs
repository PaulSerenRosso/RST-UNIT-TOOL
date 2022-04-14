using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    private GridManager _gridManager;

    private void OnEnable()
    {
        _gridManager = (GridManager) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Update Grid"))
        {
            if (_gridManager.SizeGrid.Equals(float3.zero))
                return;
            if (_gridManager.CellCount.Equals(int3.zero))
                return;
           
            _gridManager.CellFactor = new int3(_gridManager.CellCount.y * _gridManager.CellCount.z,
                _gridManager.CellCount.z, 1);
            _gridManager.SizeCells = new float3(_gridManager.SizeGrid.x / _gridManager.CellCount.x,
                _gridManager.SizeGrid.y / _gridManager.CellCount.y, _gridManager.SizeGrid.z / _gridManager.CellCount.z);
            _gridManager.Grid.Clear();

            for (int x = 0; x < _gridManager.CellCount.x; x++)
            {
                for (int y = 0; y < _gridManager.CellCount.y; y++)
                {
                    for (int z = 0; z < _gridManager.CellCount.z; z++)
                    {
                        GridCell cell = new GridCell();
                        cell.MinPosition =
                            (float3) _gridManager.transform.position + new float3(x * _gridManager.SizeCells.x,
                                y * _gridManager.SizeCells.y, _gridManager.SizeCells.z * z);
                        cell.MaxPosition = cell.MinPosition + _gridManager.SizeCells;
                        cell.CenterPosition = cell.MinPosition + _gridManager.SizeCells / 2;
                        _gridManager.Grid.Add(cell);
                        cell.id = _gridManager.Grid.Count - 1; 
                    }
                }
            }
        }
        EditorUtility.SetDirty(target);
    }
}