using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;


public class Enemy : MonoBehaviour
{

    [SerializeField]
    private FSMEnemy _fsm;
    public Pathfinding _pathfinding;


    [SerializeField]
    public List<Node> _pathPatrol = new List<Node>();

    [SerializeField]
    public Node targetNodePosition;


    public bool investigating = false;

    [SerializeField]
    private FOV _fov;

    [SerializeField]
    private float _patrolSpeed = 4;
    [SerializeField]
    private float _chaseSpeed = 5;
    [SerializeField]
    private float _pathFindSpeed = 4.5f;
    private Vector3 _velocity;
    [SerializeField]
    private float _maxForce;

    [SerializeField]
    private float obstacleRaycastLength = 0.7f;

    public FOV Fov { get { return _fov; } }

    [SerializeField]
    private bool DEBUG;

    void Start()
    {
        _pathfinding = new Pathfinding();
        _fsm = new FSMEnemy();
        _fsm.AddStates(EnemyStates.Patrol, new PatrolNodesState(_fsm, this, _pathPatrol, _patrolSpeed));
        _fsm.AddStates(EnemyStates.Pathfinding, new PathfindingState(_fsm, this, _pathFindSpeed));
        _fsm.AddStates(EnemyStates.Chase, new ChaseState(_fsm, this, _chaseSpeed));
        _fsm.ChangeState(EnemyStates.Patrol);

    }
    void Update()
    {
        _fsm.Update();
    }



    public Vector3 Seek(Vector3 target)
    {
        Vector3 desired = target - transform.position;
        desired.Normalize();
        desired *= _patrolSpeed;
        Debug.DrawLine(transform.position, transform.position + desired, Color.gray);

        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce * Time.deltaTime);

        return steering;
    }

    public void AddForce(Vector3 force, float maxSpeed)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, maxSpeed);
    }

    public void Move()
    {
        _velocity = new Vector3(_velocity.x, _velocity.y, 0);  //FORCE 2D
        transform.position += _velocity * Time.deltaTime;
        if (_velocity.sqrMagnitude > 0.000001f)
            transform.up = _velocity;
    }

    public Vector3 ObstacleAvoidance()
    {
        TryGetObstacleAvoidance(out Vector3 steering);
        return steering;
    }

    public bool TryGetObstacleAvoidance(out Vector3 steering)
    {
        steering = Vector3.zero;

        Vector3 rightOrigin = transform.position + transform.right * 0.7f;
        Vector3 leftOrigin = transform.position - transform.right * 0.7f;

        if (DEBUG)
        {
            Debug.DrawLine(rightOrigin, rightOrigin + transform.up * obstacleRaycastLength);
            Debug.DrawLine(leftOrigin, leftOrigin + transform.up * obstacleRaycastLength);
        }

        if (Physics.Raycast(rightOrigin, transform.up, obstacleRaycastLength, _fov.WallLayer))
        {
            // Obstáculo a la derecha: desviarse a la izquierda.
            steering = Seek(transform.position - transform.right);
            return true;
        }
        else if (Physics.Raycast(leftOrigin, transform.up, obstacleRaycastLength, _fov.WallLayer))
        {
            // Obstáculo a la izquierda: desviarse a la derecha.
            steering = Seek(transform.position + transform.right);
            return true;
        }

        return false;
    }

}
