using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(PlayerManager))]
public class PlayerManagerEditor : Editor
{
    private PlayerManager _playerManager;
    private void OnEnable()
    {
        _playerManager = (PlayerManager)target;
        
        //set chaque element
    }

    public override void OnInspectorGUI()
    {
        
        base.OnInspectorGUI();
        if(GUILayout.Button("Save Player Type"))
        {
            if(_playerManager.AllTypesPlayer.Count != 0)
            EnumCreator.WritePlayerType(_playerManager.AllTypesPlayer);
        }
        
    }
}
