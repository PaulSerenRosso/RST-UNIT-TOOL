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

        if (GUILayout.Button("Update Material on Units"))
        {
            for (int i = 0; i < _playerManager.AllPlayersList.Count; i++)
            {
                for (int j = 0; j < _playerManager.AllPlayersList[i].AllSquads.Count; j++)
                {
                    for (int k = 0; k < _playerManager.AllPlayersList[i].AllSquads[j].AllUnits.Count; k++)
                    {
                        for (int l = 0; l < _playerManager.AllPlayersList[i].AllSquads[j].AllUnits[k].Units.Count; l++)
                        {
                        Debug.Log(i);
                        _playerManager.AllPlayersList[i].AllSquads[j].AllUnits[k].Units[l].GetComponent<MeshRenderer>().material = _playerManager.AllPlayersList[i].Material;
                            
                        }
                    }
                }
            }
        }
        
    }
}
