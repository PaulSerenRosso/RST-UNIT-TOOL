using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UnitsBoidJobsData
{
    [Serializable]
    public class UnitsBoidJobsClass
    {
        public UnitScript Unit;
        public List<List<UnitScript>> Units;
        public float[] Speeds;
        public float3 OldVelocity;
        public float SmoothFactor;
    }

    public struct Base
    {
        public float Speed;
        public float3 ResultVector;
    }

    public struct Movment
    {
        public Base Base;
        public float3 Position;

        public Movment(float speed, float3 position)
        {
            Position = position;
            Base.Speed = speed;
            Base.ResultVector = float3.zero;
        }
    }

    public struct Alignement
    {
        public Base Base;
        public float3 Rotation;

        public Alignement(float speed, float3 rotation)
        {
            Base.Speed = speed;
            Rotation = rotation;
            Base.ResultVector = float3.zero;
        }
    }
}