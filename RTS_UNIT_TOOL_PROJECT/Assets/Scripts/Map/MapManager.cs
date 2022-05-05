using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class MapManager : MonoBehaviour
{
    [SerializeField] public List<TerrainY> AllTerrains;

    [SerializeField] public Material UnderSkyMaterial;
    public TerrainData BaseData;
    [SerializeField] private float _detectDistanceNavMeshPoint;


    public static MapManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject); // Suppression d'une instance précédente (sécurité...sécurité...)

        Instance = this;
        UnderSkyMaterial.color = new Color(0, 0, 0, 0);
    }

    private void OnValidate()
    {
        for (int i = 0; i < AllTerrains.Count; i++)
        {
            Vector3 pos = new Vector3(AllTerrains[i].Terrain.transform.position.x, AllTerrains[i].YPosition,
                AllTerrains[i].Terrain.transform.position.z);
            AllTerrains[i].Terrain.transform.position = pos;
            AllTerrains[i].SizeYTerrain = AllTerrains[i].YMaxPosition - AllTerrains[i].YPosition;
            AllTerrains[i].YDistances.Clear();
            for (int j = 0; j < AllTerrains.Count; j++)
            {
                if (AllTerrains[i] != AllTerrains[j])
                {
                    AllTerrains[i].YDistances.Add(new TerrainYDistance(AllTerrains[j].MovmentType,
                         AllTerrains[j].YPosition-AllTerrains[i].YPosition));
                }
            }
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
            Vector3 center = size + new Vector3(AllTerrains[i].Terrain.transform.position.x,
                AllTerrains[i].YMaxPosition, AllTerrains[i].Terrain.transform.position.z);
            Gizmos.DrawCube(center,
                new Vector3(AllTerrains[i].Terrain.terrainData.size.x, 0.5f,
                    AllTerrains[i].Terrain.terrainData.size.z));
        }
    }

    public bool UpdateDestinationWithTerrainNav(float3 aimPosition, int index, out NavMeshHit navHit)
    {
        RaycastHit hit;
        float3 origin = new float3(aimPosition.x, AllTerrains[index].YMaxPosition, aimPosition.z);
        navHit = new NavMeshHit();
        if (index == 1)
        {
            if (NavMesh.SamplePosition(aimPosition, out navHit, _detectDistanceNavMeshPoint,
                AllTerrains[index].NavArea))
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
        {
            float _distance = 0;
            for (int i = 0; i < AllTerrains[index].YDistances.Count; i++)
            {
                if (AllTerrains[index].YDistances[i].Type == UnitMovmentType.Earthly)
                {
                    _distance = AllTerrains[index].YDistances[i].YDistance;
                }
            }

            if (NavMesh.SamplePosition((float3) Vector3.up * _distance + aimPosition, out navHit,
                _detectDistanceNavMeshPoint, AllTerrains[index].NavArea))
            {
                return true;
            }
            else
            {
                Debug.LogWarning("Navemesh Dont find");
                return false;
            }
        }
    }
}