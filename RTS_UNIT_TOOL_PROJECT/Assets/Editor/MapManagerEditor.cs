using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NavMeshBuilder = UnityEditor.AI.NavMeshBuilder;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    private MapManager _mapManager;
    private void OnEnable()
    {
        _mapManager =(MapManager) target;
  
    }

    private void OnDisable()
    {
   
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Update All Terrain"))
        {
            List<TerrainData> newTerrainDatas = new List<TerrainData>();
            for (int i = 0; i < _mapManager.AllTerrains.Count; i++)
            {
             AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_mapManager.AllTerrains[i].Terrain.terrainData));    
            }
            AssetDatabase.SaveAssets();
            string basePath = AssetDatabase.GetAssetPath(_mapManager.BaseData); 
            for (int i = 0; i < 3; i++)
            {   
                string currentPath = "";
                switch (i)
                {
                    case 0:
                    {
                        currentPath = "Assets/Terrain/Sky.asset"; 
                        break;
                    }
                    case 1:
                    {
                        currentPath = "Assets/Terrain/Land.asset"; 
                        break;
                    }
                    case 2:
                    {
                        currentPath = "Assets/Terrain/UnderGround.asset"; 
                        break;
                    }
                }
              
                AssetDatabase.CopyAsset(basePath,
                    currentPath);
                TerrainData currentTerrainData = (TerrainData) AssetDatabase.LoadAssetAtPath(currentPath, typeof(TerrainData)); 
                newTerrainDatas.Add(currentTerrainData);
            }
            AssetDatabase.SaveAssets();
            for (int i = 0; i < newTerrainDatas.Count; i++)
            {
                
                _mapManager.AllTerrains[i].Terrain.terrainData = newTerrainDatas[i];
                _mapManager.AllTerrains[i].Terrain.gameObject.GetComponent<TerrainCollider>().terrainData =
                    newTerrainDatas[i];

            }
            EditorUtility.SetDirty(target);

            // suppre les data
            // que j'en duplique 3 
            // que ensuite je les assigne à la liste et au terrain 
            // puis enfin il faut que je bake
        }

        if (GUILayout.Button("Update GridCells With Terrain"))
        {
           
            GridManager grid = FindObjectOfType<GridManager>();
            for (int i = 0; i < _mapManager.AllTerrains.Count; i++)
            {
         
              int currentMinLine = Mathf.FloorToInt((_mapManager.AllTerrains[i].YPosition-grid.transform.position.y) / grid.SizeCells.y);
              int currentMaxLine = Mathf.FloorToInt((_mapManager.AllTerrains[i].YMaxPosition-grid.transform.position.y) / grid.SizeCells.y);
              currentMaxLine += 1;
              
              int _maxID = (currentMaxLine)*grid.CellCount.z+grid.CellCount.z;
             int _minID = (currentMinLine)*grid.CellCount.z+grid.CellCount.z-grid.CellCount.z;
      
             for (int k = 0; k < grid.CellCount.x ; k++)
             {
                 int _finalMaxID = (k) * (grid.CellCount.y * grid.CellCount.z) + _maxID;
                 int _finalMinID = (k) * (grid.CellCount.y * grid.CellCount.z) + _minID;
            
                 for (int j = _finalMinID; j < _finalMaxID; j++)
                 {
                     if(grid.Grid[j].AllUnits == null)
                     grid.Grid[j].AllUnits = new List<UnitMovmentList>();
                     UnitMovmentList unitMovmentList = new UnitMovmentList();
                     unitMovmentList.MovmentType = _mapManager.AllTerrains[i].MovmentType;
              
                     
                     grid.Grid[j].AllUnits.Add(unitMovmentList);
                  
                     grid.Grid[j].Color += _mapManager.AllTerrains[i].Color;
                 }
             }
            }
            

         
        }
    }
}
