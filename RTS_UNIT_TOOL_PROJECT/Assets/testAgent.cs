using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class testAgent : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Vector3 move;
    // Update is called once per frame
    private void Start()
    {
    agent.SetDestination(transform.position+Vector3.forward*10+Vector3.right*10);
    

    }

  
}
