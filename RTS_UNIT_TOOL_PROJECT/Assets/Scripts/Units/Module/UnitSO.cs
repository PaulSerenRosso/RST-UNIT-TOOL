using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "UnitSO", menuName = "Data/UnitSO", order = 1)]
public class UnitSO : ScriptableObject
{

    #region Base
    [Header("Base")]
    public UnitType Type;
    public float MaxHealth;
    public UnitMovmentType MovmentType;
    #endregion
    
    #region Speeds

    [Header("Speeds")] public float DestinationPointSpeed;
    public float DestinationPointRotationSpeed; 
    public float DestinationFightSpeed;
    public float DestinationFightRotationSpeed;
  [HideInInspector]
   public List<int> MovmentTypeList= new List<int>();

   public float BoidSmoothFactor;
    
    #endregion

    #region Detection Fight

    [Header("Engagement")] 
    public List<UnitMovmentType> MovmentTypesDetection;
    public DistanceCellsClass DistanceEngagement;
    public bool IsEngageAutomatically;
    [HideInInspector]
    public List<int> MovmentTypeIndicesDetection = new List<int>();

    public bool WithStopMovment;
    public DistanceSquare DistanceStopMovment;
    
    
    #endregion
    
    #region Targeting

    [Header("Targeting")] public TargetChange ChangePriority;
    public List<TargetPriorityClass> TargetPriorities;


    #endregion

    #region Attack

    [Header("Attacks")] public bool IsRandomDamage;
    public float MinDamage;
    public float MaxDamage;
    public float BaseDamage;
    public float CooldownAttack;
    public DistanceSquare DistanceAttack;
    #endregion

    #region Boids 

      [Header("Boids")]
        [SerializeField] private BoidParameter _cohesionParameter;
        [SerializeField] private BoidParameter _alignementParameter;
        [SerializeField] private BoidParameter _avoidanceParameter;
        [HideInInspector]
        public DistanceCellsClass[] AllDistanceCellsClass = new DistanceCellsClass[4];
        public float DistanceToDestinationPoint;
        [HideInInspector]
       public float[] AllSpeeds = new float[3]; 

    #endregion
  

   public void OnValidate()
   {
      BoidSmoothFactor = Mathf.Clamp(BoidSmoothFactor, 0, 1); 
       MovmentTypeList.Clear();
       MovmentTypeList.Add((int)MovmentType);
       MovmentTypeIndicesDetection.Clear();
       for (int i = 0; i < MovmentTypesDetection.Count; i++)
       {
           MovmentTypeIndicesDetection.Add((int)MovmentTypesDetection[i]);
       }
       DistanceStopMovment.BaseToSquare();
       GridManager gridManager = FindObjectOfType<GridManager>();
       DistanceAttack.BaseToSquare();
       DistanceEngagement.ConvertDistanceToDistanceCell(gridManager);
       _alignementParameter.Distance.Base =
           Mathf.Min(_alignementParameter.Distance.Base, _cohesionParameter.Distance.Base);
       _avoidanceParameter.Distance.Base =
           Mathf.Min(_avoidanceParameter.Distance.Base, _cohesionParameter.Distance.Base);   
       _cohesionParameter.Distance.ConvertDistanceToDistanceCell(gridManager);
       _alignementParameter.Distance.ConvertDistanceToDistanceCell(gridManager);
       _avoidanceParameter.Distance.ConvertDistanceToDistanceCell(gridManager);
       
       AllDistanceCellsClass[0].SetValues(_cohesionParameter.Distance.Base, _cohesionParameter.Distance.DistanceJob);
       AllDistanceCellsClass[1].SetValues(_alignementParameter.Distance.Base, _alignementParameter.Distance.DistanceJob);
       AllDistanceCellsClass[2].SetValues(_avoidanceParameter.Distance.Base, _avoidanceParameter.Distance.DistanceJob);
    
       AllSpeeds[0] = _cohesionParameter.Speed;
       AllSpeeds[1] = _alignementParameter.Speed;
       AllSpeeds[2] = _avoidanceParameter.Speed;
   
   }
}
