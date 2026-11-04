using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;


public class Enemy : MonoBehaviour
{

    [SerializeField]
    private FSMEnemy _fsm;
    public Pathfinding _pathfinding;

    private int _currentNodeIndex = 0;

    [SerializeField]
    public List<Node> _pathPatrol = new List<Node>();

    [SerializeField]
    public Node targetNodePosition;

    [SerializeField]
    public Node playerNodePosition; //TEMPORAL, es para mockear el nodo donde está el player, en un futuro se va a calcular con el player y el grid

    public bool investigating = false;

    [SerializeField]
    private FOV _fov;

    [SerializeField]
    private float _maxSpeed;
    private Vector3 _velocity;
    [SerializeField]
    private float _maxForce;

    public FOV Fov { get { return _fov; } }

    void Start()
    {
        _pathfinding = new Pathfinding();
        _fsm = new FSMEnemy();
        _fsm.AddStates(EnemyStates.Patrol, new PatrolNodesState(_fsm, this, _pathPatrol, _maxSpeed));
        _fsm.AddStates(EnemyStates.Pathfinding, new PathfindingState(_fsm, this));
        _fsm.AddStates(EnemyStates.Chase, new ChaseState(_fsm, this));
        _fsm.ChangeState(EnemyStates.Patrol);

    }
    // Update is called once per frame
    void Update()
    {
        //SI EL PLAYER ESTA DENTRO DEL FOV, CAMBIAR A CHASESTATE

        //ELSE IF LLEGA EL EVENT QUE ALGUIEN VIO AL JUGADOR EN TAL NODO (Mockeado con la tecla espacio) CAMBIA DE ESTADO A PATHFINDING
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            targetNodePosition = playerNodePosition;
            investigating = true;
            _fsm.ChangeState(EnemyStates.Pathfinding);
        }
        //EN CHASE STATE, SI EL PLAYER SALE DEL FOV, CAMBIAR A PATHFINDINGSTATE PARA QUE VUELVA A LA POSICION DE PATROL




        _fsm.Update();
    }



    public Vector3 Seek(Vector3 target)
    {
        Vector3 desired = target - transform.position;
        desired.Normalize();
        desired *= _maxSpeed;
        Debug.DrawLine(transform.position, transform.position + desired, Color.gray);

        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce * Time.deltaTime);

        return steering;
    }

    public void AddForce(Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, _maxSpeed);
    }

    public void Move()
    {
        _velocity = new Vector3(_velocity.x, _velocity.y, 0);  //FORCE 2D
        transform.position += _velocity * Time.deltaTime;
        if (_velocity.sqrMagnitude > 0.000001f)
            transform.up = _velocity;
    }
}
