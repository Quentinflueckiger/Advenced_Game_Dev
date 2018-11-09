﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class AgentBehaviour : MonoBehaviour {


    public Transform target;
    NavMeshAgent _agent;


    // Use this for initialization
    void Start () {
       _agent = GetComponent<NavMeshAgent>();     
	}
	
	// Update is called once per frame
	void Update () {
        _agent.SetDestination(target.position);
    }
}
