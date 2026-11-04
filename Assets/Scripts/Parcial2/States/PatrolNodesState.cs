using UnityEngine;
using System.Collections.Generic;

public class PatrolNodesState : States
{
    private Enemy _enemy;
    private List<Node> _pathPatrol;
    private float _patrolSpeed;
    private int _currentNodeIndex = 0;

    public PatrolNodesState(FSMEnemy f, Enemy _enemy, List<Node> path, float patrolSpeed)
    {
        this.fsm = f;
        this._enemy = _enemy;
        this._pathPatrol = path;
        this._patrolSpeed = patrolSpeed;
    }


    public override void OnEnter()
    {
        _enemy.Fov.OnPlayerDetected += OnPlayerDetected;
        
        _enemy.investigating = false;

        if (_pathPatrol.Contains(_enemy.targetNodePosition))
        {
            _currentNodeIndex = _pathPatrol.IndexOf(_enemy.targetNodePosition);

        }
        // ESTOY CERCA DEL pathpatrol
        if (Vector3.Distance(_enemy.transform.position, _pathPatrol[_currentNodeIndex].transform.position) < 0.1f)
        {
            Debug.Log("Estoy cerca de la zona de patrulla, patrullo");
            _currentNodeIndex = _pathPatrol.IndexOf(_enemy.targetNodePosition);
            // SÍ. Continúo patrullando
            return;
        }

        Debug.Log("No estoy cerca del target node, cambio a pathfinding");
        //el nodo mas cercano del enemigo al path
        _enemy.targetNodePosition = _pathPatrol[_currentNodeIndex];
        fsm.ChangeState(EnemyStates.Pathfinding);
    }


    public override void OnUpdate()
    {

        

        if (_enemy != null && _pathPatrol != null && _pathPatrol.Count != 0)
        {
            Movement();
        }

    }
    public override void OnExit()
    {
        _enemy.Fov.OnPlayerDetected -= OnPlayerDetected;
    }

    public void Movement()
    {

        if (_pathPatrol == null || _pathPatrol.Count == 0)
        {
            return;

        }

        Node nextNode = _pathPatrol[_currentNodeIndex];

        _enemy.transform.position = Vector3.MoveTowards(
            _enemy.transform.position,
            nextNode.transform.position,
            Time.deltaTime * _patrolSpeed
        );
        _enemy.transform.up = (nextNode.transform.position - _enemy.transform.position).normalized;

        if (Vector3.Distance(_enemy.transform.position, nextNode.transform.position) < 0.1f)
        {

            _currentNodeIndex++;

            if (_currentNodeIndex >= _pathPatrol.Count)
            {
                _currentNodeIndex = 0;
            }

        }
    }

    private void OnPlayerDetected(bool isDetected)
    {
        if (isDetected)
        {
            Debug.Log("Player detected! Switching to ChaseState.");
            fsm.ChangeState(EnemyStates.Chase);
        }
    }

}
