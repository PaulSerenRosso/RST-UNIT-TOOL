using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UnitModuleManager : MonoBehaviour
{
    public List<UnitModule> AllModules;
    public List<UnitModule> CurrentModules;
    public List<UnitModule> StartModules;
    public List<UnitModule> EndModules;

    public void OnValidate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;
        StartModules.Clear();
        for (int i = 0; i < AllModules.Count; i++)
        {
     StartModules.Add(AllModules[i]);
        }
    }
    public void OnStart()
    {
        
    }
    
    public void OnUpdate()
    {
        for (int i = 0; i < StartModules.Count; i++)
        {
            StartModules[i].OnStart();
        }
        StartModules.Clear();
        for (int i = 0; i < CurrentModules.Count; i++)
        {
            CurrentModules[i].OnUpdate();
        }

        for (int i = 0; i < EndModules.Count; i++)
        {
            EndModules[i].OnEnd();
        }
        EndModules.Clear();
    }
}
