using UnityEngine;
using System.Collections.Generic;

public class PatrolNodesState : States
{
    private Enemy _enemy;
    private List<Node> _path;
    private int _patrolSpeed = 5;
    private int _currentNodeIndex = 0;
    private int _lastNodeIndex = 0;

    public PatrolNodesState(FSMEnemy f, Enemy _enemy, List<Node> path)
    {
        this.fsm = f;
        this._enemy = _enemy;
        this._path = path;
    }


    public override void OnEnter()
    {
        // ESTOY CERCA DEL target node?
        if (Vector3.Distance(_enemy.transform.position, _path[_lastNodeIndex].transform.position) < 0.1f)
        {
            Debug.Log("Estoy cerca de la zona de patrulla, patrullo");
            // SÍ. Continúo patrullando
            return;
        }

        Debug.Log("No estoy cerca del target node, cambio a pathfinding");
        //el nodo mas cercano del enemigo al path
        _enemy.targetNodePosition = _path[_lastNodeIndex];
        fsm.ChangeState(EnemyStates.Pathfinding);
    }


    public override void OnUpdate()
    {

        if (_enemy != null && _path != null && _path.Count != 0)
        {
            Movement();
        }

    }
    public override void OnExit()
    {
        _lastNodeIndex = _currentNodeIndex;
    }

    public void Movement()
    {

        if (_path == null || _path.Count == 0)
        {
            return;

        }

        Node nextNode = _path[_currentNodeIndex];

        _enemy.transform.position = Vector3.MoveTowards(
            _enemy.transform.position,
            nextNode.transform.position,
            Time.deltaTime * _patrolSpeed
        );

        if (Vector3.Distance(_enemy.transform.position, nextNode.transform.position) < 0.1f)
        {

            _currentNodeIndex++;

            if (_currentNodeIndex >= _path.Count)
            {
                _currentNodeIndex = 0;
            }

        }
    }

}
