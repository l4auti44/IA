using UnityEngine;
using System.Collections.Generic;

public class PathfindingState : States
{

    private int _chaseSpeed = 8;
    private Enemy _enemy;
    private List<Node> _path;
    private Node startNode;
    private int _currentNodeIndex = 0;

    private bool _isReturning = false;

    public PathfindingState(FSMEnemy f, Enemy _enemy, List<Node> _path)
    {
        this.fsm = f;
        this._enemy = _enemy;
        this._path = _path;
        
    }
    public override void OnEnter()
    {
        if(_enemy != null && _path != null && _path.Count != 0)
        {
            
            var currentDistance = float.MaxValue;
            foreach (Node node in _path)
            {
                float distance = Vector3.Distance(_enemy.transform.position, node.transform.position);
                if (distance < currentDistance)
                {
                    currentDistance = distance;
                    startNode = node;
                    Debug.Log("Start Node: " + startNode.name);
                }
            }
            if (_enemy != null && startNode != null)
            {
                _path = _enemy._pathfinding.AStar(startNode, _enemy.playerNodePosition);
            }
        }
    }

    public override void OnExit()
    {
        _currentNodeIndex = 0;
    }

    public override void OnUpdate()
    {
        
        
            if(_path != null && _path.Count > 0)
            {
                MovementChase();
            }
        
    }

    public void MovementChase()
    {

        if (_path == null || _path.Count == 0)
        {
            return;
        }
        
        Node nextNode = _path[_currentNodeIndex];

        _enemy.transform.position = Vector3.MoveTowards(
            _enemy.transform.position,
            nextNode.transform.position,
            Time.deltaTime * _chaseSpeed
        );

        if (Vector3.Distance(_enemy.transform.position, nextNode.transform.position) < 0.1f)
        {

            _currentNodeIndex++;

            if (_currentNodeIndex >= _path.Count)
            {
                if (!_isReturning)
                {
                    
                    _path.Reverse();
                    _currentNodeIndex = 0;
                    _isReturning = true;
                }
                else
                {
                    
                    fsm.ChangeState(EnemyStates.Patrol);
                }
            }
            
        }

        
    }

    

}
