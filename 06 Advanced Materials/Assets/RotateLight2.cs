using UnityEngine;
using System.Collections;

public class RotateLight2 : MonoBehaviour {

    public Transform target;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(target.position, Vector3.forward, 30 * Time.deltaTime);
    }
}
