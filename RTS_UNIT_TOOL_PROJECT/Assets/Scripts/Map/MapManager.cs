using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField]
   public List<TerrainY> AllTerrains;

   public TerrainData BaseData;

   
       public static MapManager Instance { get; private set; }
 
       void Awake()
       {
           if (Instance != null && Instance != this)
               Destroy(gameObject);    // Suppression d'une instance précédente (sécurité...sécurité...)
 
           Instance = this;
       }

       
   
    private void OnValidate()
    {
        for (int i = 0; i < AllTerrains.Count; i++)
        {
            Vector3 pos = new Vector3(AllTerrains[i].Terrain.transform.position.x,AllTerrains[i].YPosition ,AllTerrains[i].Terrain.transform.position.z) ;
            AllTerrains[i].Terrain.transform.position = pos;
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < AllTerrains.Count; i++)
                {
                    Gizmos.color = AllTerrains[i].Color;
                    Gizmos.DrawSphere(AllTerrains[i].Terrain.transform.position, 0.5f);
                    Vector3 size = AllTerrains[i].Terrain.terrainData.size / 2;
                    size.y = 0; 
                    Vector3 center = size+ new Vector3(AllTerrains[i].Terrain.transform.position.x,AllTerrains[i].YMaxPosition,AllTerrains[i].Terrain.transform.position.z);
                    Gizmos.DrawCube(center, new Vector3(AllTerrains[i].Terrain.terrainData.size.x, 0.5f, AllTerrains[i].Terrain.terrainData.size.z) );
                   
                }
    }
}
