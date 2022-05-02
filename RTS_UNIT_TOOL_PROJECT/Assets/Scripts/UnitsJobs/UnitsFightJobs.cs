using System.Collections.Generic;
using Unity.Burst;

using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

public class UnitsFightJobs : MonoBehaviour
{
    public static UnitsFightJobs Instance;


    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject); // Suppression d'une instance précédente (sécurité...sécurité...)

        Instance = this;
    }

    void OnUpdate()
    {
        
    }
    
    
    [BurstCompile]
    struct   GetUnitDistanceAmount:  IJobParallelFor
    {
        public void Execute(int index)
        {
            
        }
    }
    [BurstCompile]
    public struct GetRandomDamageAmount: IJob
    {
        public float Min;
        public float Max;
        public float DamageValue;
        public void Execute()
        {
          DamageValue = Random.Range(Min, Max);
        }
    }


}
