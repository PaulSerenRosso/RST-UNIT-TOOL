using System;


public class UnitJobData
{
    [Serializable]
    public struct Base
    {
        public UnitModule module;
        public int index;
    }

    public struct UnitsNeighboursCell
    {
        public Base BaseData;
    }
    
    public struct UnitsDistance
    {
        public Base BaseData;
    }
    
    public struct TransformUnits
    {
        public Base BaseData;
    }
    
    
    public struct UnitsBool
    {
        public Base BaseData;
    }
    
    
}





