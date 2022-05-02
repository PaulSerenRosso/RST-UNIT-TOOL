using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            DestroyOldUnits(grid);
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

        if (GUILayout.Button("Add Squad to Player Squad"))
        {
            UpdatePlayerSquad();
        }
    }

    void UpdatePlayerSquad()
    {
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();
        bool isSamePlayer = false;
        for (int i = 0; i < playerManager.AllPlayersList.Count; i++)
        {
            if (playerManager.AllPlayersList[i].AllSquads.Contains(_squad))
            {
                if (_squad.Player != playerManager.AllPlayersList[i].Name)
                {
                    playerManager.AllPlayersList[i].AllSquads.Remove(_squad);
                   
                }
                else
                {
                     isSamePlayer = true;
                }

                break;
            }
        }

        if (!isSamePlayer)
        {
            for (int j = 0; j < playerManager.AllPlayersList.Count; j++)
            {
                if (_squad.Player == playerManager.AllPlayersList[j].Name)
                {
                    playerManager.AllPlayersList[j].AllSquads.Add(_squad);
                    break;
                }
            }
       
        }
     
    }

    void DestroyOldUnits(GridManager gridManager)
    {
        for (int j = 0; j < _squad.AllUnits.Count; j++)
        {


            for (int i = _squad.AllUnits[j].Units.Count - 1; i > -1; i--)
            {

                Debug.Log("test");
                Transform unitTransform = _squad.transform.GetChild(i);
                UnitScript unitScript = unitTransform.GetComponent<UnitScript>();
                gridManager.Grid[unitScript.Cell.ID].AllUnits[unitScript.MovementCellIndexList].Units.Remove(unitScript);
              
                DestroyImmediate(unitTransform.gameObject);
            }
        }

        _squad.AllUnits.Clear();
        _squad.movmentTypeUnitsIndex.Clear();
    }

    void InstantiateNewUnits()
    {
  
        _squad.transform.position = new Vector3(_squad.transform.position.x, _squad.transform.position.y,
            _squad.transform.position.x);
        for (int i = 0; i < _squad.SpawnerUnits.Count; i++)
        {
            UnitScript unitPrefab = _squad.SpawnerUnits[i].unitObject.GetComponent<UnitScript>();
            if(!_squad.movmentTypeUnitsIndex.Contains((int) unitPrefab.SO.MovmentType))
            { 
                _squad.movmentTypeUnitsIndex.Add((int)unitPrefab.SO.MovmentType);
                UnitMovmentList _unitList = new UnitMovmentList();
                _unitList.Units = new List<UnitScript>();
                _unitList.MovmentType = unitPrefab.SO.MovmentType;
                _squad.AllUnits.Add(_unitList);
            }
           
            for (int j = 0; j < _squad.SpawnerUnits[i].Count; j++)
            {
                GameObject unit =
                    (GameObject) PrefabUtility.InstantiatePrefab(_squad.SpawnerUnits[i].unitObject, _squad.transform);
                for (int k = 0; k < _squad.AllUnits.Count; k++)
                {
                    if (_squad.AllUnits[i].MovmentType == unitPrefab.SO.MovmentType)
                    {
                        _squad.AllUnits[i].Units.Add(unit.GetComponent<UnitScript>()); 
                        break;
                    }
                }
               
            }
        }
    }

    void SetUnitsVariables(MapManager map, GridManager grid)
    {
        for (int i = 0; i < _squad.AllUnits.Count; i++)
        {
            for (int j = 0; j < _squad.AllUnits[i].Units.Count; j++)
            { 
                UnitScript unitScript = _squad.AllUnits[i].Units[j];
            SetUnitPosition(unitScript, map);
            SetUnitCell(unitScript, grid);
            EditorUtility.SetDirty(unitScript);
            }
        }
    }

    void SetUnitPosition(UnitScript unitScript, MapManager map)
    {
        Debug.Log((int) unitScript.SO.MovmentType);
        Debug.Log(map.AllTerrains[(int) unitScript.SO.MovmentType]);
        Vector2 randomPosition = Random.insideUnitCircle * _squad.SpawnAreasSize[(int) unitScript.SO.MovmentType];
        unitScript.transform.position = new Vector3(unitScript.transform.position.x + randomPosition.x,
            unitScript.transform.position.y,
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
        int idCell = (finalCellCount.x) * grid.CellCount.y * grid.CellCount.z +
                     (finalCellCount.y) * grid.CellCount.z + finalCellCount.z;

        Debug.Log(idCell);
        unitScript.Cell = grid.Grid[idCell];

        if (unitScript.Cell.TryGetIndexList(unitScript.SO.MovmentType, out int index))
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