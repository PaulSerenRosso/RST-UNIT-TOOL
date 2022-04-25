using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BoidSO", menuName = "Data/BoidSO", order = 1)]
public class BoidSO : ScriptableObject
{
    [SerializeField] private BoidParameter _cohesionParameter;
    [SerializeField] private BoidParameter _alignementParameter;
    [SerializeField] private BoidParameter _avoidanceParameter; 
    public float SpeedDestination;
    public DistanceCellsClass DistanceUnitEndDestination;
    public float DistanceEndDestination;
    

    public DistanceCellsClass[] AllDistanceCellsClass = new DistanceCellsClass[4];

   [HideInInspector]
   public float[] AllSpeeds = new float[3]; 

   public void OnValidate()
   {   
       GridManager gridManager = FindObjectOfType<GridManager>();
       _cohesionParameter.Distance.ConvertDistanceToDistanceCell(gridManager);
       _alignementParameter.Distance.ConvertDistanceToDistanceCell(gridManager);
       _avoidanceParameter.Distance.ConvertDistanceToDistanceCell(gridManager);
       DistanceUnitEndDestination.ConvertDistanceToDistanceCell(gridManager);
       AllDistanceCellsClass[0].SetValues(_cohesionParameter.Distance.Base, _cohesionParameter.Distance.DistanceJob);
       AllDistanceCellsClass[1].SetValues(_alignementParameter.Distance.Base, _alignementParameter.Distance.DistanceJob);
       AllDistanceCellsClass[2].SetValues(_avoidanceParameter.Distance.Base, _avoidanceParameter.Distance.DistanceJob);
       AllDistanceCellsClass[3].SetValues(DistanceUnitEndDestination.Base, DistanceUnitEndDestination.DistanceJob);
       AllSpeeds[0] = _cohesionParameter.Speed;
       AllSpeeds[1] = _alignementParameter.Speed;
       AllSpeeds[2] = _avoidanceParameter.Speed;
   
   }
}
