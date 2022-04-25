using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class MapManager : MonoBehaviour
{
    [SerializeField]
   public List<TerrainY> AllTerrains;

   [SerializeField] public Material UnderSkyMaterial;
   public TerrainData BaseData;
   [SerializeField] private float _detectDistanceNavMeshPoint;

   public float CameraRayLength;
       public static MapManager Instance { get; private set; }
 
       void Awake()
       {
           if (Instance != null && Instance != this)
               Destroy(gameObject);    // Suppression d'une instance précédente (sécurité...sécurité...)
 
           Instance = this;
           UnderSkyMaterial.color = new Color(0, 0, 0, 0);
       }

    private void OnValidate()
    {
        for (int i = 0; i < AllTerrains.Count; i++)
        {
            
            Vector3 pos = new Vector3(AllTerrains[i].Terrain.transform.position.x,AllTerrains[i].YPosition ,AllTerrains[i].Terrain.transform.position.z) ;
            AllTerrains[i].Terrain.transform.position = pos;
            AllTerrains[i].SizeYTerrain = AllTerrains[i].YMaxPosition - AllTerrains[i].YPosition;
            float currentDistanceToCamera = AllTerrains[i].YPosition - Camera.main.transform.position.y;
            if (currentDistanceToCamera >= CameraRayLength)
            CameraRayLength = currentDistanceToCamera;
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

 public bool UpdateDestinationWithTerrainNav(float3 aimPosition, int index, out NavMeshHit navHit )
 {
     RaycastHit hit;
     float3 origin = new float3(aimPosition.x, AllTerrains[index].YMaxPosition, aimPosition.z);
     Ray ray = new Ray(origin, Vector3.down);
     navHit = new NavMeshHit();
     if (index == 1)
     { 
         if (NavMesh.SamplePosition(aimPosition, out navHit, _detectDistanceNavMeshPoint, AllTerrains[index].NavArea))
         {
             return true;
         }
         else
         {
             Debug.LogWarning("Navemesh Dont find");
             return false;
                
         }
         
     }
     if(Physics.Raycast(ray, out hit, AllTerrains[index].SizeYTerrain, AllTerrains[index].LayerMask))
        {
            if (NavMesh.SamplePosition(hit.point, out navHit, _detectDistanceNavMeshPoint, AllTerrains[index].NavArea))
            {
                return true;
            }
            else
            {
                Debug.LogWarning("Navemesh Dont find");
                return false;
                
            }
        }
        else
        {Debug.LogError("terrain not detect");
            return false;
            
        }
     
    }
}
