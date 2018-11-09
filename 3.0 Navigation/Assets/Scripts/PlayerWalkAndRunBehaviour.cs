﻿using UnityEngine;

public class PlayerWalkAndRunBehaviour : MonoBehaviour {

    public float turnSpeed=360;
    public float forwardSpeed=1.0f;

    private Rigidbody _rigidbody;   //Reference to the characters rigidbody
    private Animator _animator;     //Reference to thecharactersanimator
    private Transform _cam;         //Reference to the main camera of scene
    private Vector3 _moveDir;       //Move direction derived from inputs
    private float _turnAmount;      //Amount of forward movement
    private float _forwardAmount;   //Amount of turn movement

	// Use this for initialization
	void Start () {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();

        // Avoid rotation on rigidbody. The capsule will be always upright.    
        _rigidbody.constraints =    RigidbodyConstraints.FreezeRotationX |  
                                    RigidbodyConstraints.FreezeRotationY |
                                    RigidbodyConstraints.FreezeRotationZ;
        // We will use the camera's orientation to determine the walk direction 
        // The main camera object must be tagged with the tag MainCamera     
        if (Camera.main != null	) 
            _cam = Camera.main.transform; 
        else 
            Debug.LogError("Warning: no main camera found.", gameObject);
    }

    private void FixedUpdate()
    {
        // Read inputs 
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        // Calculate move direction from the camera orientation 
        _moveDir = inputV * _cam.forward * forwardSpeed + inputH * _cam.right;

        // We scale down the move vector for walking 
        if (Input.GetKey(KeyCode.LeftShift)) _moveDir *= 0.5f;

        Move(_moveDir);
    }

    private void Move(Vector3 moveDir)
    {
        // Convert the move vector from world to local coords.	    
        moveDir = transform.InverseTransformDirection(moveDir);
        // Take the angle between the z-axis and the x-axis as turn amount    
        _turnAmount = Mathf	.Atan2(moveDir.x, moveDir.z);	 

        // Take the z-axis as forward amount 
        _forwardAmount = moveDir.z;

        // Apply rotation because the rigidbody rotation are freezed  
        // The position will be set by physics later in the OnAnimatorMove method 
        transform.Rotate(0, _turnAmount * turnSpeed * Time.deltaTime, 0);

        // Update the animator parameters 
        _animator.SetFloat("Forward", _forwardAmount, 0.1f, Time.deltaTime);
        _animator.SetFloat("Turn", _turnAmount, 0.1f, Time.deltaTime);
    }

    public void OnAnimatorMove()
    {
        if (Time.deltaTime > 0)
        {
            // On ground we calculate the speed from the root motion 
            Vector3 v = _animator.deltaPosition / Time.deltaTime;
            // We preserve the existing vertical part of the current velocity	        
            v.y = _rigidbody.velocity.y; 

            _rigidbody.velocity = v;
        }
    }


}
