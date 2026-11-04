using UnityEngine;

public class ChaseState : States
{
    private int _chaseSpeed = 8;
    private FSMEnemy _fsm;
    private Enemy _enemy;
    private FOV _fov;

    public ChaseState(FSMEnemy fsm, Enemy enemy)
    {
        _enemy = enemy;
        _fov = _enemy.Fov;
        this._fsm = fsm;
    }
    public override void OnEnter()
    {
        _fov.OnPlayerDetected += OnPlayerUnDetected;
        _enemy.GetComponent<Renderer>().material.color = Color.red;
    }

    public override void OnExit()
    {
        _fov.OnPlayerDetected -= OnPlayerUnDetected;
        _enemy.GetComponent<Renderer>().material.color = Color.magenta;
    }

    public override void OnUpdate()
    {
        _enemy.AddForce(_enemy.Seek(_fov.target.position));
        _enemy.Move();
    }

    private void OnPlayerUnDetected(bool isDetected)
    {
        if (!isDetected)
        {
            Debug.Log("Player lost! Switching to PathfindingState.");
            // CONSIDERAR ESPERAR UNOS SEGUNDOS
            _fsm.ChangeState(EnemyStates.Patrol);
        }
    }

}
