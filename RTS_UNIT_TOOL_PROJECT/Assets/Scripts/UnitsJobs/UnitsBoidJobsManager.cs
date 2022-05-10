using System.Collections.Generic;
using System.Data;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

public class UnitsBoidJobsManager : MonoBehaviour
{
    public List<UnitsBoidJobsData.UnitsBoidJobsClass> BoidsData;
    public int jobSize;

    public static UnitsBoidJobsManager Instance { get; private set; }
 
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);    // Suppression d'une instance précédente (sécurité...sécurité...)
 
        Instance = this;
    }
    public void OnUpdate()
    {
    
    
        if (BoidsData.Count == 0)
            return;

        #region SetVariables

        JobHandle _handle = new JobHandle();

        NativeArray<MinMaxIndex> _minMaxIndicesCohesion =
            new NativeArray<MinMaxIndex>(BoidsData.Count, Allocator.TempJob);
        NativeArray<MinMaxIndex> _minMaxIndicesAlignement =
            new NativeArray<MinMaxIndex>(BoidsData.Count, Allocator.TempJob);
        NativeArray<MinMaxIndex> _minMaxIndicesAvoidance =
            new NativeArray<MinMaxIndex>(BoidsData.Count, Allocator.TempJob);
        NativeArray<UnitsBoidJobsData.Movment> _avoidanceMovments =
            new NativeArray<UnitsBoidJobsData.Movment>(BoidsData.Count, Allocator.TempJob);
        NativeArray<UnitsBoidJobsData.Movment> _cohesionMovments =
            new NativeArray<UnitsBoidJobsData.Movment>(BoidsData.Count, Allocator.TempJob);
        NativeArray<UnitsBoidJobsData.Alignement> _alignementRotation =
            new NativeArray<UnitsBoidJobsData.Alignement>(BoidsData.Count, Allocator.TempJob);

        List<List<float3>> _allUnits = new List<List<float3>>()
        {
            new List<float3>(), new List<float3>(), new List<float3>()
        };
        for (int i = 0; i < BoidsData.Count; i++)
        {
            _cohesionMovments[i] =
                new UnitsBoidJobsData.Movment(BoidsData[i].Speeds[0], BoidsData[i].Unit.transform.position);
            _alignementRotation[i] =
                new UnitsBoidJobsData.Alignement(BoidsData[i].Speeds[1], BoidsData[i].Unit.transform.position);
            _avoidanceMovments[i] =
                new UnitsBoidJobsData.Movment(BoidsData[i].Speeds[2], BoidsData[i].Unit.transform.position);
            List<MinMaxIndexClass> currentMinMaxIndices = new List<MinMaxIndexClass>()
            {
                new MinMaxIndexClass(), new MinMaxIndexClass(), new MinMaxIndexClass()
            };
           
            for (int j = 0; j < 3; j++)
            {
                currentMinMaxIndices[j].MinIndex = _allUnits[j].Count;
         
                for (int k = 0; k < BoidsData[i].Units[j].Count; k++)
                {
                    if (j == 1)
                    {
                        
                        _allUnits[j].Add(BoidsData[i].Units[j][k].transform.forward);
                    }

                    else
                    {
                        _allUnits[j].Add(BoidsData[i].Units[j][k].transform.position);
                        
                    }
                }

                currentMinMaxIndices[j].MaxIndex = _allUnits[j].Count;
            }
            _minMaxIndicesCohesion[i] = new MinMaxIndex(currentMinMaxIndices[0]);
            _minMaxIndicesAlignement[i] = new MinMaxIndex(currentMinMaxIndices[1]);
            _minMaxIndicesAvoidance[i] = new MinMaxIndex(currentMinMaxIndices[2]);
        }

        #endregion

        #region SetJobs

        float3[] _velocity = new float3[BoidsData.Count];
        float3[] _torque = new float3[BoidsData.Count];
        if (_allUnits[0].Count != 0)
        {
//            Debug.Log(_allUnits[0].Count);
            NativeArray<float3> _allUnitsCohesion = new NativeArray<float3>(_allUnits[0].Count, Allocator.TempJob);
            for (int i = 0; i < _allUnits[0].Count; i++)
                _allUnitsCohesion[i] = _allUnits[0][i];
            GetCohesionMovment _getCohesionMovment = new GetCohesionMovment()
            {
                MovmentsArray = _cohesionMovments, UnitsPosition = _allUnitsCohesion,
                MinMaxIndices = _minMaxIndicesCohesion
            };
            _handle = _getCohesionMovment.Schedule(BoidsData.Count, jobSize);
            _handle.Complete();
            for (int i = 0; i < _getCohesionMovment.MovmentsArray.Length; i++)
            {
                   _velocity[i] += _getCohesionMovment.MovmentsArray[i].Base.ResultVector;
                
            }
         
            _allUnitsCohesion.Dispose();
            
        }

        if (_allUnits[1].Count != 0)
        {
            NativeArray<float3> _allUnitsAlignement = new NativeArray<float3>(_allUnits[1].Count, Allocator.TempJob);
            for (int i = 0; i < _allUnits[1].Count; i++)
                _allUnitsAlignement[i] = _allUnits[1][i];
            GetAlignementRotation _getAlignementRotation = new GetAlignementRotation()
            {
                AlignementArray = _alignementRotation, UnitsRotationForward = _allUnitsAlignement,
                MinMaxIndices = _minMaxIndicesAlignement
            };
            _handle = _getAlignementRotation.Schedule(BoidsData.Count, jobSize);
            _handle.Complete();
            for (int i = 0; i < _getAlignementRotation.AlignementArray.Length; i++)
                _torque[i] += _getAlignementRotation.AlignementArray[i].Base.ResultVector;
            _allUnitsAlignement.Dispose();
        }

        
        if (_allUnits[2].Count != 0)
        {
            NativeArray<float3> _allUnitsAvoidance = new NativeArray<float3>(_allUnits[2].Count, Allocator.TempJob);
            for (int i = 0; i < _allUnits[2].Count; i++)
                _allUnitsAvoidance[i] = _allUnits[2][i];
            GetAvoidanceMovment _getAvoidanceMovment = new GetAvoidanceMovment()
            {
                MovmentsArray = _avoidanceMovments, UnitsPosition = _allUnitsAvoidance,
                MinMaxIndices = _minMaxIndicesAvoidance
            };
            _handle = _getAvoidanceMovment.Schedule(BoidsData.Count, jobSize);
            _handle.Complete();
            for (int i = 0; i < _getAvoidanceMovment.MovmentsArray.Length; i++)
            {
                    _velocity[i] += _getAvoidanceMovment.MovmentsArray[i].Base.ResultVector;
                          
            }
            
            _allUnitsAvoidance.Dispose();
        }

        #endregion

        #region Results

        for (int i = 0; i < BoidsData.Count; i++)
        {

            
            BoidsData[i].Unit.Boid.OldVelocity =
                Vector3.Lerp(BoidsData[i].OldVelocity, _velocity[i] * Time.deltaTime, BoidsData[i].SmoothFactor);
            BoidsData[i].Unit.Agent.Move( BoidsData[i].Unit.Boid.OldVelocity);
            float3 rotation = (BoidsData[i].Unit.transform.forward + (Vector3) _torque[i] * Time.deltaTime).normalized;
            if (!rotation.Equals(float3.zero))
            {
                   BoidsData[i].Unit.transform.forward = ( BoidsData[i].Unit.transform.forward + (Vector3) _torque[i] * Time.deltaTime).normalized;
            }
       
//        Debug.Log(  BoidsData[i].Unit.transform.forward );
        }
        #endregion

        #region Dispose

        _minMaxIndicesAlignement.Dispose();
        _minMaxIndicesAvoidance.Dispose();
        _minMaxIndicesCohesion.Dispose();
        _alignementRotation.Dispose();
        _cohesionMovments.Dispose();
        _avoidanceMovments.Dispose();
        BoidsData.Clear();

        #endregion

    }

    #region Jobs

    [BurstCompile]
    public struct GetCohesionMovment : IJobParallelFor
    {
        public NativeArray<UnitsBoidJobsData.Movment> MovmentsArray;
      [ReadOnly] public NativeArray<float3> UnitsPosition;
      [ReadOnly]
        public NativeArray<MinMaxIndex> MinMaxIndices;


        public void Execute(int index)
        {
            UnitsBoidJobsData.Movment movment = MovmentsArray[index];
       
            
            for (int i = MinMaxIndices[index].MinIndex; i < MinMaxIndices[index].MaxIndex; i++)
                movment.Base.ResultVector += UnitsPosition[i];

//            print(movment.Base.ResultVector);
            if (movment.Base.ResultVector.Equals(float3.zero))
                return;
       
            movment.Base.ResultVector /= MinMaxIndices[index].MaxIndex  - MinMaxIndices[index].MinIndex;
         // movment.Base.ResultVector =  Vector3.Lerp(movment.Position, movment.Base.ResultVector, 0.2f);
            movment.Base.ResultVector -= movment.Position;
            movment.Base.ResultVector =  Vector3.Normalize(movment.Base.ResultVector);
            movment.Base.ResultVector *= movment.Base.Speed;
          
            MovmentsArray[index] = movment;
        }
    }

    [BurstCompile]
    public struct GetAlignementRotation : IJobParallelFor
    {
        public NativeArray<UnitsBoidJobsData.Alignement> AlignementArray;
        [ReadOnly]
        public NativeArray<float3> UnitsRotationForward;
        [ReadOnly]
        public NativeArray<MinMaxIndex> MinMaxIndices;

        public void Execute(int index)
        {
            UnitsBoidJobsData.Alignement alignement = AlignementArray[index];
            for (int i = MinMaxIndices[index].MinIndex; i < MinMaxIndices[index].MaxIndex; i++)
                alignement.Base.ResultVector += UnitsRotationForward[i];
            if (alignement.Base.ResultVector.Equals(float3.zero))
                return;
            
            alignement.Base.ResultVector /= MinMaxIndices[index].MaxIndex - 1 - MinMaxIndices[index].MinIndex;
            alignement.Base.ResultVector *= alignement.Base.Speed;
            AlignementArray[index] = alignement;
        }
    }

    [BurstCompile]
    public struct GetAvoidanceMovment : IJobParallelFor
    {
        public NativeArray<UnitsBoidJobsData.Movment> MovmentsArray;
        [ReadOnly]
        public NativeArray<float3> UnitsPosition;
        [ReadOnly]
        public NativeArray<MinMaxIndex> MinMaxIndices;

        public void Execute(int index)
        {
            UnitsBoidJobsData.Movment movment = MovmentsArray[index];
            for (int i = MinMaxIndices[index].MinIndex; i < MinMaxIndices[index].MaxIndex; i++)
                movment.Base.ResultVector += movment.Position-UnitsPosition[i];
            if (movment.Base.ResultVector.Equals(float3.zero))
                return;
           
            movment.Base.ResultVector /= MinMaxIndices[index].MaxIndex - MinMaxIndices[index].MinIndex; 
        
          movment.Base.ResultVector =  Vector3.Normalize(movment.Base.ResultVector);
            movment.Base.ResultVector *= movment.Base.Speed;
            MovmentsArray[index] = movment;
        }
    }
    #endregion
}