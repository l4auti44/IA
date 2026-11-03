using UnityEngine;
using System.Collections.Generic;

public class PatrolNodesState : States
{
    private Enemy _enemy;
    private Pathfinding _pathfinding;
    private List<Node> _path;

    public PatrolNodesState(FiniteStateMachine f, Enemy _enemy, Pathfinding pathfinding, List<Node> path)
    {
        this._enemy = _enemy;
        this._pathfinding = pathfinding;
        this._path = path;
    }
    public override void OnEnter()
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        
    }

}
