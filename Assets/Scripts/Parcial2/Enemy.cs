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






    void Start()
    {
        _pathfinding = new Pathfinding();
        _fsm = new FSMEnemy();
        _fsm.AddStates(EnemyStates.Patrol, new PatrolNodesState(_fsm, this, _pathPatrol));
        _fsm.AddStates(EnemyStates.Pathfinding, new PathfindingState(_fsm, this));
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
            _fsm.ChangeState(EnemyStates.Pathfinding);
        }
        //EN CHASE STATE, SI EL PLAYER SALE DEL FOV, CAMBIAR A PATHFINDINGSTATE PARA QUE VUELVA A LA POSICION DE PATROL




        _fsm.Update();
    }
}
