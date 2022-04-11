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
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(AllTerrains[i].Terrain.transform.position, 3f);
                }
    }
}
