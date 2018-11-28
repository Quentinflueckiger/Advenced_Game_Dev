using UnityEngine;
using System.Collections;

public class RotateLight : MonoBehaviour {

    public Transform target;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(target.position, Vector3.up, 30 * Time.deltaTime);
    }
}
