using UnityEngine;
using System.Collections.Generic;
using Patterns.Observer.Struct_EventManager;
using System;

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
        EventManager.Subscribe<PlayerFoundData>(InvestigatePos);



        _enemy.investigating = false;

        if (_pathPatrol.Contains(_enemy.targetNodePosition))
        {
            _currentNodeIndex = _pathPatrol.IndexOf(_enemy.targetNodePosition);

        }
        // Busco primero con Line of Sight si estoy cerca de un nodo del pathPatrol
        if (CheckIfNearPatrolNode()) return;

        //sino veo ningun nodo, pathfind

        Debug.Log("No estoy cerca del target node");
        //PAthfinding al nodo mas cercano del path
        //el nodo mas cercano del enemigo al path
        _enemy.targetNodePosition = _pathPatrol[_currentNodeIndex];
        fsm.ChangeState(EnemyStates.Pathfinding);

    }



    private bool CheckIfNearPatrolNode()
    {
        foreach (Node node in _pathPatrol)
        {
            bool isOnLineOfSight = _enemy.Fov.LineOfSight(node.gameObject);
            if (isOnLineOfSight)
            {
                _currentNodeIndex = _pathPatrol.IndexOf(node);
                Debug.Log("Estoy viendo al nodo de patrulla: " + node.name);
                return true;
            }
        }
        return false;
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
        EventManager.Unsubscribe<PlayerFoundData>(InvestigatePos);

    }

    public void Movement()
    {

        if (_pathPatrol == null || _pathPatrol.Count == 0)
        {
            return;

        }

        Node nextNode = _pathPatrol[_currentNodeIndex];

        IsPatrolNodeVisible(nextNode);
  
        _enemy.AddForce(_enemy.Seek(nextNode.transform.position), _patrolSpeed);
        _enemy.Move();

        if (Vector3.Distance(_enemy.transform.position, nextNode.transform.position) < 0.1f)
        {

            _currentNodeIndex++;

            if (_currentNodeIndex >= _pathPatrol.Count)
            {
                _currentNodeIndex = 0;
            }

        }
    }

    private void IsPatrolNodeVisible(Node nextNode)
    {
        float distance = Vector3.Distance(_enemy.transform.position, nextNode.transform.position);
        //raycast para ver si hay pared
        if (Physics.Raycast(_enemy.transform.position, nextNode.transform.position - _enemy.transform.position, out RaycastHit hit, distance, _enemy.Fov.WallLayer))
        {
            Debug.DrawLine(_enemy.transform.position, hit.point, Color.red);
            //si hay pared, pathfind al nodo
            _enemy.targetNodePosition = nextNode;
            fsm.ChangeState(EnemyStates.Pathfinding);
            return;
        }
        else
        {
            Debug.DrawLine(_enemy.transform.position, nextNode.transform.position, Color.green);
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
    private void InvestigatePos(PlayerFoundData data)
    {
        _enemy.investigating = true;
        _enemy.targetNodePosition = data.playerClosestNode;
        fsm.ChangeState(EnemyStates.Pathfinding);
    }

}
