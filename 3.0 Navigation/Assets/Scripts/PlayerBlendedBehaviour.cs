using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlendedBehaviour : MonoBehaviour {

    private Animator _animator;

	// Use this for initialization
	void Start () {

        _animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        float walkOrRun = Input.GetKey(KeyCode.LeftShift) ? 0.5f : 1.0f;
        _animator.SetFloat("Forward", Input.GetAxis("Vertical") * walkOrRun);
        _animator.SetFloat("Turn", Input.GetAxis("Horizontal"));
    }
}
