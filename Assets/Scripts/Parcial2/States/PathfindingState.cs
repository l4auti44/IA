using UnityEngine;
using System.Collections.Generic;

public class PathfindingState : States
{

    private int _chaseSpeed = 8;
    private Enemy _enemy;
    private List<Node> _path;
    private Node startNode;
    private int _currentNodeIndex = 0;



    public PathfindingState(FSMEnemy f, Enemy _enemy)
    {
        this.fsm = f;
        this._enemy = _enemy;

    }
    public override void OnEnter()
    {
        FindStartNode();
    }

    private void FindStartNode()
    {
        if (_enemy != null && GameManager.Instance != null && GameManager.Instance.nodes.Count != 0)
        {

            var currentDistance = float.MaxValue;
            foreach (Node node in GameManager.Instance.nodes)
            {
                float distance = Vector3.Distance(_enemy.transform.position, node.transform.position);
                if (distance < currentDistance)
                {
                    currentDistance = distance;
                    startNode = node;
                    //Debug.Log("Start Node: " + startNode.name);
                }
            }
            if (_enemy != null && startNode != null)
            {
                _path = _enemy._pathfinding.AStar(startNode, _enemy.targetNodePosition);
            }
        }
    }

    public override void OnExit()
    {
        _currentNodeIndex = 0;
    }

    public override void OnUpdate()
    {
        // SI EL PLAYER ESTA DENTRO DEL FOV, CAMBIAR A CHASESTATE



        if (_path != null && _path.Count > 0)
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

                // LLEGUE
                fsm.ChangeState(EnemyStates.Patrol);
            }

        }


    }



}
