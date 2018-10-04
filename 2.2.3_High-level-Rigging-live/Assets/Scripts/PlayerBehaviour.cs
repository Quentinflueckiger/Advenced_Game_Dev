using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    public float transitionTime = 0.3f;
    private Animator _animator;
    private float _waveWeight = 0.0f;
    private float _deltaT;


	// Use this for initialization
	void Start () {

        _animator = GetComponent<Animator>();
        _deltaT = 1.0f / transitionTime;
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetKey("up"))
            _animator.SetInteger("AnimParam", 1);
        else
            _animator.SetInteger("AnimParam", 0);

        if (Input.GetKey("space"))
        {
            if (_waveWeight < 1.0f) _waveWeight = Mathf.Clamp(_waveWeight + _deltaT * Time.deltaTime, 0.0f, 1.0f);
            _animator.SetLayerWeight(1, _waveWeight);
        }
        else
        {
            if (_waveWeight > 0.0f) _waveWeight = Mathf.Clamp(_waveWeight - _deltaT * Time.deltaTime, 0.0f, 1.0f);
            _animator.SetLayerWeight(1, _waveWeight);
        }
	}
}
