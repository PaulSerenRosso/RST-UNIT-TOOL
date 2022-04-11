using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(Squad))]
public class SquadEditor : Editor
{
    private Squad _squad;
//système de spawn
    void OnEnable()
    {
        _squad = (Squad) target;
    }

    public override void OnInspectorGUI()
    {
        
        base.OnInspectorGUI();
        Debug.Log(_squad.AllUnits.Count);
        GUILayout.Label("Update Squad when it's put in the scene");
        if (GUILayout.Button("Update Squad"))
        {
            Debug.Log(_squad.transform.childCount);
       
            for (int i =_squad.transform.childCount-1;i > 1; i--)
            {
                Transform unitTransform = _squad.transform.GetChild(i);
               DestroyImmediate(unitTransform.gameObject); 
               Debug.Log("testst");
            }
            
            // lock la position des 
            _squad.AllUnits.Clear();
            _squad.transform.position = new Vector3(_squad.transform.position.x, _squad.transform.position.y,
                _squad.transform.position.x);
            for (int i = 0; i < _squad.SpawnerUnits.Count; i++)
            {
                for (int j = 0; j <_squad.SpawnerUnits[i].Count ; j++)
                {
                 GameObject unit =(GameObject) PrefabUtility.InstantiatePrefab(_squad.SpawnerUnits[i].unitObject,_squad.transform);
                }
            }
            for (int i = 0; i < _squad.transform.childCount; i++)
            {
                Transform unitTransform = _squad.transform.GetChild(i);
                UnitScript unitScript = unitTransform.GetComponent<UnitScript>();
                unitScript.Squad = _squad;
                _squad.AllUnits.Add(unitScript);
          
               
            }

            EditorUtility.SetDirty(target);
        }
    }
}