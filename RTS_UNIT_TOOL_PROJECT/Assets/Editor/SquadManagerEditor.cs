using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(SquadManager))]
public class SquadManagerEditor : Editor
{
    private SquadManager _squadManager;
    private void OnEnable()
    {
        _squadManager = (SquadManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Save Player Type"))
        {
            if(_squadManager.AllTypesUnit.Count != 0)
            EnumCreator.WritePlayerType(_squadManager.AllTypesUnit);
            
        }
    }
}
