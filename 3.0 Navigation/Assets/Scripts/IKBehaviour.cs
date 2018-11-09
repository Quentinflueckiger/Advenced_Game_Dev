using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKBehaviour : MonoBehaviour
{
    private Animator _animator;

    public bool doFootIK = true;
    public float feetOffsetY = 0.12f;
    public Vector3 footLeftOffsetEulerAngles = new Vector3(-6, -5, 0);
    public Vector3 footRightOffsetEulerAngles = new Vector3(-6, 8, 0);

    public Transform lookAtPoint;
    public float lookIKWeight = 0.0f;
    public float lookIKBodyWeight = 0.3f;
    public float lookIKHeadWeight = 0.9f;
    public float lookIKEyesWeight = 1.0f;
    public float lookIKClampWeight = 0.5f;

    public bool handIsHolding = false;
    public Transform handRightHandle;
    public float handGrabSpeed = 2.0f;

    private bool _handDoGrabOrRelease;
    private float _handGrabWeight = 0.0f;
    private float _handKeyDownTime;

    private Transform _footL;
    private Transform _footR;
    private Vector3 _footL_HitPos;
    private Vector3 _footR_HitPos;
    private Quaternion _footL_HitRot;
    private Quaternion _footR_HitRot;
    private float _feetIKWeight = 0.0f;

    void Start()
    {
        _animator = GetComponent<Animator>();
        // Get the feet transforms from the animator 
        _footL = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        _footR = _animator.GetBoneTransform(HumanBodyBones.RightFoot);

        // Initialize the hit rotation so they are not null 
        _footL_HitRot = _footL.rotation;
        _footR_HitRot = _footR.rotation;
    }

    void Update() {
        GetFeetHits();
    }

    void OnAnimatorIK() {
        DoFootIK();
        DoLookIK();
        DoHandIK();
    }

    void GetFeetHits()
    {
        // get animator variables 
        float forward = _animator.GetFloat("Forward");
        float turn = _animator.GetFloat("Turn");
        bool isGrounded = _animator.GetBool("OnGround");

        // Only do foot IK standing still 
        _feetIKWeight = doFootIK && (forward + turn) < 0.05f && isGrounded ? 1.0f : 0.0f;
        if (_feetIKWeight == 1.0)
        {
            // Do a ray cast for the left foot in world space downwards 
            RaycastHit hit;
            Vector3 posWS = _footL.TransformPoint(Vector3.zero);
            if (Physics.Raycast(posWS, -Vector3.up, out hit, 1))
            {
                _footL_HitPos = hit.point + new Vector3(0, feetOffsetY, 0);
                _footL_HitRot = Quaternion.FromToRotation(transform.up, hit.normal) * 
                                Quaternion.Euler(footLeftOffsetEulerAngles) * 
                                transform.rotation;
            }

            // Do a ray cast for the right foot in world space downwards            
            posWS = _footR.TransformPoint(Vector3.zero);
            if (Physics.Raycast(posWS, -Vector3.up, out hit, 1)) 
            {
                _footR_HitPos = hit.point + new Vector3(0, feetOffsetY, 0);
                _footR_HitRot = Quaternion.FromToRotation(transform.up, hit.normal) * 
                                Quaternion.Euler(footRightOffsetEulerAngles) * 
                                transform.rotation;
            }
        }
    }

    void DoFootIK()
    {
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _feetIKWeight);
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _feetIKWeight);
        _animator.SetIKPosition(AvatarIKGoal.LeftFoot, _footL_HitPos);
        _animator.SetIKPosition(AvatarIKGoal.RightFoot, _footR_HitPos);

        _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _feetIKWeight);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _feetIKWeight);
        _animator.SetIKRotation(AvatarIKGoal.LeftFoot, _footL_HitRot);
        _animator.SetIKRotation(AvatarIKGoal.RightFoot, _footR_HitRot);
    }

    void DoLookIK()
    {
        _animator.SetLookAtWeight(lookIKWeight, lookIKBodyWeight, lookIKHeadWeight, lookIKEyesWeight, lookIKClampWeight);
        _animator.SetLookAtPosition(lookAtPoint.position);
    }

    void DoHandIK()
    {
        if (Input.GetKeyDown(KeyCode.G) && Time.time > _handKeyDownTime + 0.5f)
        {
            handIsHolding = !handIsHolding;
            _handKeyDownTime = Time.time;   // keep time to avoid double firing events 
        }

        if (handIsHolding && handRightHandle != null)
        {   // Lerp in the grab weight 
            _handGrabWeight = Mathf.Lerp(_handGrabWeight, 1.0f,
                                         Time.deltaTime * handGrabSpeed);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _handGrabWeight);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _handGrabWeight);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, handRightHandle.position); _animator.SetIKRotation(AvatarIKGoal.RightHand, handRightHandle.rotation);
        }
        else
        {
            if (_handGrabWeight > 0.01f)
            {   // Lerp out the grap weight 
                _handGrabWeight = Mathf.Lerp(_handGrabWeight, 0.0f,
                                             Time.deltaTime * handGrabSpeed);
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _handGrabWeight);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _handGrabWeight);
                _animator.SetIKPosition(AvatarIKGoal.RightHand, handRightHandle.position);
                _animator.SetIKRotation(AvatarIKGoal.RightHand, handRightHandle.rotation);
            }
            else
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }
        }
    }

}

