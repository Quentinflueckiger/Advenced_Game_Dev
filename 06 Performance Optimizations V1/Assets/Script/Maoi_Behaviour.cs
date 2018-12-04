using UnityEngine;
using System.Collections;
using UnityEngine.Profiling;

public class Maoi_Behaviour : MonoBehaviour
{
    struct myStruct
    {
        public float x, y, z;
    }

    private Transform _lookAt;
    private Transform _myTransform;
    private float _myY;

    void Start()
    {
        _lookAt = GameObject.Find("Main Camera").transform;
        _myTransform = transform;
        _myY = _myTransform.position.y;
    }

    // Update is called once per frame 
    void Update()
    {
        /*Transform lookAt = GameObject.Find("Main Camera").transform;
        transform.LookAt(new Vector3(lookAt.position.x, transform.position.y, lookAt.position.z));
        */
        _myTransform.LookAt(new Vector3(_lookAt.position.x,
                                        _myY,
                                        _lookAt.position.z));
        transform.Rotate(270, 90, 0);


        Profiler.BeginSample("Expensive Loop");
        for (int i = 0; i < 1000; i++)
        {
            myStruct a;
            a.x = 1.0f;
            a.y = 1.0f;
            a.z = 1.0f;
        }
        Profiler.EndSample();
    }
}
