using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AI;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEditor.AI.NavMeshBuilder;
using Object = UnityEngine.Object;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    private MapManager _mapManager;
    private void OnEnable()
    {
        _mapManager =(MapManager) target;
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
       NavMeshBuilder.BuildNavMesh();


            // suppre les data
            // que j'en duplique 3 
            // que ensuite je les assigne à la liste et au terrain 
            // puis enfin il faut que je bake
        }
    }
}
