using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PathfindingState : States
{

    private Enemy _enemy;
    private List<Node> _path;
    private Node startNode;
    private int _currentNodeIndex = 0;

    private float _pathFindSpeed;



    public PathfindingState(FSMEnemy f, Enemy _enemy, float pathFindSpeed)
    {
        this.fsm = f;
        this._enemy = _enemy;
        this._pathFindSpeed = pathFindSpeed;
    }
    public override void OnEnter()
    {
        _enemy.Fov.OnPlayerDetected += OnPlayerDetected;
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

                if (!_enemy.investigating)
                {
                    foreach (Node node in _path)
                    {
                        if (_enemy._pathPatrol.Contains(node))
                        {
                            //cut path to the node that is in the patrol path
                            int index = _path.IndexOf(node);
                            _path = _path.Take(index + 1).ToList();
                            _enemy.targetNodePosition = node;
                            break;
                        }
                    }
                }
            }
        }
    }

    public override void OnExit()
    {
        _currentNodeIndex = 0;
        _enemy.Fov.OnPlayerDetected -= OnPlayerDetected;
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

        /*
        _enemy.transform.position = Vector3.MoveTowards(
            _enemy.transform.position,
            nextNode.transform.position,
            Time.deltaTime * _chaseSpeed
        );

        _enemy.transform.up = (nextNode.transform.position - _enemy.transform.position).normalized;
        */
        _enemy.AddForce(_enemy.Seek(nextNode.transform.position), _pathFindSpeed);
        _enemy.Move();
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

    private void OnPlayerDetected(bool isDetected)
    {
        if (isDetected)
        {
            Debug.Log("Player detected! Switching to ChaseState.");
            fsm.ChangeState(EnemyStates.Chase);
        }
    }



}
