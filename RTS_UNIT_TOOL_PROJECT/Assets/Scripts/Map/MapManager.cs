using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private List<TerrainY> allTerrainY;


    private void OnValidate()
    {
        for (int i = 0; i < allTerrainY.Count; i++)
        {
            Vector3 pos = new Vector3(allTerrainY[i].Terrain.position.x,allTerrainY[i].YPosition ,allTerrainY[i].Terrain.position.z) ;
            allTerrainY[i].Terrain.position = pos;
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < allTerrainY.Count; i++)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(allTerrainY[i].Terrain.position, 3f);
                }
    }
}
