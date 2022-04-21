using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CustomEditor(typeof(Squad))]
public class SquadEditor : Editor
{
    private Squad _squad;
//système de spawn

    void OnEnable()
    {
        _squad = (Squad) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Update Squad"))
        {
            MapManager map = FindObjectOfType<MapManager>();
            GridManager grid = FindObjectOfType<GridManager>();
            DestroyOldUnits();
            InstantiateNewUnits();
            SetUnitsVariables(map, grid);
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Update Squad Position"))
        {
            MapManager map = FindObjectOfType<MapManager>();
            GridManager grid = FindObjectOfType<GridManager>();
            SetUnitsVariables(map, grid);
            EditorUtility.SetDirty(target);
        }
        
    }

    void DestroyOldUnits()
    {
        for (int i = _squad.AllUnits.Count-1; i > -1; i--)
        {
            Debug.Log("test");
            Transform unitTransform = _squad.transform.GetChild(i);
            UnitScript unitScript = unitTransform.GetComponent<UnitScript>();
            unitScript.Cell.AllUnits[unitScript.MovementCellIndexList].Units.Remove(unitScript);
            DestroyImmediate(unitTransform.gameObject);
        }
    }

    void InstantiateNewUnits()
    {
        _squad.AllUnits.Clear();
        _squad.transform.position = new Vector3(_squad.transform.position.x, _squad.transform.position.y,
            _squad.transform.position.x);
        for (int i = 0; i < _squad.SpawnerUnits.Count; i++)
        {
            for (int j = 0; j < _squad.SpawnerUnits[i].Count; j++)
            {
                GameObject unit =
                    (GameObject) PrefabUtility.InstantiatePrefab(_squad.SpawnerUnits[i].unitObject, _squad.transform);
                _squad.AllUnits.Add(unit.GetComponent<UnitScript>());
            }
        }
    }

    void SetUnitsVariables(MapManager map, GridManager grid)
    {
        for (int i = 0; i < _squad.AllUnits.Count; i++)
        {
            UnitScript unitScript = _squad.AllUnits[i];
            SetUnitPosition(unitScript, map);
            SetUnitCell( unitScript, grid);
            EditorUtility.SetDirty(unitScript);
        }
    }

    void SetUnitPosition(UnitScript unitScript, MapManager map)
    {
        Vector2 randomPosition = Random.insideUnitCircle * _squad.SpawnAreasSize[(int) unitScript.MovmentType];
       unitScript.transform.position = new Vector3(unitScript.transform.position.x + randomPosition.x,
            map.AllTerrains[(int) unitScript.MovmentType].Terrain.transform.position.y,
            unitScript.transform.position.z + randomPosition.y);
        unitScript.Squad = _squad;
     
    }

    void SetUnitCell(UnitScript unitScript, GridManager grid)
    {
        float3 cellCount = (unitScript.transform.position - grid.transform.position) / grid.SizeCells;

        cellCount.x = Mathf.FloorToInt(cellCount.x);
        cellCount.y = Mathf.FloorToInt(cellCount.y);
        cellCount.z = Mathf.FloorToInt(cellCount.z);
  
 
        int3 finalCellCount = (int3) cellCount;
        int idCell = (finalCellCount.x ) * grid.CellCount.y * grid.CellCount.z +
            (finalCellCount.y ) * grid.CellCount.z + finalCellCount.z ;
    
        Debug.Log(idCell);
        unitScript.Cell = grid.Grid[idCell];
        
        if (unitScript.Cell.TryGetIndexList(unitScript.MovmentType, out int index))
        {
              if (unitScript.Cell.AllUnits[index].Units == null)
                        unitScript.Cell.AllUnits[index].Units = new List<UnitScript>();
                    
                    unitScript.Cell.AllUnits[index].Units.Add(unitScript);
                    unitScript.MovementCellIndexList = index;
                 
           
        }
        else
        {
            Debug.LogError("the movment type cell isn't valid");
        }
     

      
    }
}