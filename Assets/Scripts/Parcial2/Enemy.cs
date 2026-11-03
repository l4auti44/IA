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
    private List<Node> _path = new List<Node>();
    
    [SerializeField]
    public Node playerNodePosition;


    void Start()
    {
        _pathfinding = new Pathfinding();
        _fsm = new FSMEnemy();
        _fsm.AddStates(EnemyStates.Patrol, new PatrolNodesState(_fsm, this, _path));
        _fsm.AddStates(EnemyStates.Pathfinding, new PathfindingState(_fsm, this, _path));
        _fsm.ChangeState(EnemyStates.Patrol);
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _fsm.ChangeState(EnemyStates.Pathfinding);
        }
        _fsm.Update();
    }
    
}
