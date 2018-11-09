using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class AgentAIBehaviour : MonoBehaviour
{

    public enum State
    {
        Patrolling,
        Chasing,
        Searching
    }

    public AgentSight sight;
    public float chaseSpeed = 6.0f;
    public float alertLevelDecreaseTime = 10.0f;
    public PatrolPath path;
    public float patrolSpeed = 3.5f;
    public float reachedDistance = 1.2f;

    private NavMeshAgent _agent;
    private State _state = State.Patrolling;
    private float _alertLevel = 0.0f;
    private Transform _player;
    private Vector3 _lastPositionOfInterest;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void OnEnable() { sight.playerSpotted += PlayerSpotted; }
    void OnDisable() { sight.playerSpotted -= PlayerSpotted; }
    void PlayerSpotted(Vector3 position)
    {
        _state = State.Chasing;
        _lastPositionOfInterest = position;
    }


    void Update()
    {
        switch (_state)
        {
            case State.Patrolling: Patrol(); break;
            case State.Chasing: Chase(); break;
            case State.Searching: Search(); break;
        }

    }

    void Patrol()
    {
        _agent.speed = patrolSpeed;
        Vector3 dest = path.currentWaypoint.transform.position;
        _agent.SetDestination(dest);

        // Set the next destination point if we are closer than a threshold          
        if (Vector3.Distance(dest, transform.position) < reachedDistance)
            //path.SetNextWaypointIndex(); 
            path.Next();

        // Reset the look direction to the z-axis 
        sight.LookForward();
    }
    void Chase()
    {
        // if player is not in sight and we arrived at the last position he was seen at 
        // then switch to searching the area	 
        if (!sight.playerInSight && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            _state = State.Searching;
            return;

        }

        // always look at the current player position while chasing     
        sight.LookAtPosition(_player.position); 

        // update speed 
        _agent.speed = chaseSpeed;

        // update agent destination 
        _agent.SetDestination(_lastPositionOfInterest);

        // keep alert level on maximum 
        _alertLevel = 1.0f;
    }
    void Search()
    {
        // set speed to a lerp between chase and patrol speed based on alert level 
        _agent.speed = patrolSpeed + _alertLevel * (chaseSpeed - patrolSpeed);
        // decrease alert level over time 
        if (alertLevelDecreaseTime > 0.0f)
            _alertLevel -= Time.deltaTime / alertLevelDecreaseTime;
        else _alertLevel = 0.0f;

        if (_alertLevel <= 0.0f)
        {
            _alertLevel = 0.0f;
            _state = State.Patrolling;
        }

        // keep looking at the current target     
        sight.LookAtPosition(_lastPositionOfInterest);
        _agent.SetDestination(_lastPositionOfInterest); 
    }

    void OnDrawGizmos()
    {
        if (sight == null) return;

        // project the world space head direction onto the xz-plane (normale = up-axis) 
        Vector3 headDirProj = Vector3.ProjectOnPlane(sight.headLookWS, Vector3.up);
        // add a half view angle rotation to center the view cone around the up-axis 
        Quaternion rot = Quaternion.AngleAxis(-0.5f * sight.viewAngle, Vector3.up);
        // lerp the color based on alert level 
        Handles.color = Color.Lerp(new Color(0.0f, 1.0f, 0.0f, 0.2f),
                                   new Color(1.0f, 0.0f, 0.0f, 0.2f), _alertLevel);
        Handles.DrawSolidArc(transform.position, Vector3.up, rot * headDirProj, sight.viewAngle, sight.viewRadius);
    }



}

