using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class SimpleWalk : MonoBehaviour
{

    public float walkSpeed = 8.0f;

    public float runSpeed = 18.0f;

    public float jumpSpeed = 8.0f;

    public float gravity = 20.0f;

    public float airControlFactor = 0.2f;

    public float walkAccel = 40.0f;
    public float runAccel = 70.0f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 acceleration;
    private CharacterController controller;

    private float yaw;
    private float pitch;
    public float mouseSensitivity = 400.0f;

    public float brakeDrag = 0.5f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        yaw = transform.localRotation.eulerAngles.y;
        pitch = transform.localRotation.eulerAngles.x;
        if (pitch > 180) pitch -= 360.0f;

        acceleration = new Vector3(0.0f, -gravity, 0.0f);
    }

    void FixedUpdate()
    {
        // mouse look
        if (Input.GetKey(KeyCode.Mouse0))
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            while (yaw < 0.0f) yaw += 360.0f;
            yaw %= 360;

            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(yaw, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(pitch, Vector3.right);
        }

        // walk
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        moveDir.Normalize();
        moveDir = Quaternion.AngleAxis(yaw, Vector3.up) * moveDir;

        float maxSpeed = walkSpeed;
        float accelMag = walkAccel;

        // get the actual velocity of the character controller
        velocity = controller.velocity;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            maxSpeed = runSpeed;
            accelMag = runAccel;
        }

        acceleration.x = moveDir.x * accelMag;
        acceleration.z = moveDir.z * accelMag;

        // we're in the air
        if (!controller.isGrounded && airControlFactor > 0.0f)
        {
            // air control
            acceleration.x *= airControlFactor;
            acceleration.z *= airControlFactor;
        }
        else
        {
            // jump impulse
            if (Input.GetKey(KeyCode.Space))
            {
                velocity.y = jumpSpeed;
            }
            // ground 'drag' if no movement is happening
            if (moveDir.sqrMagnitude == 0.0f)
            {
                float drag = (1.0f - brakeDrag);
                velocity.x *= drag;
                velocity.z *= drag;
            }
        }

        velocity += acceleration * Time.deltaTime;


        float velocityMag = velocity.x * velocity.x +
            velocity.z * velocity.z;
        // cap velocity
        if (velocityMag > maxSpeed)
        {
            velocity.x = velocity.x / velocityMag * maxSpeed;
            velocity.z = velocity.z / velocityMag * maxSpeed;
        }

        controller.Move(velocity * Time.deltaTime);
    }

}