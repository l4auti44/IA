using UnityEngine;

public class ChaseState : States
{
    private FSMEnemy _fsm;
    private Enemy _enemy;
    private FOV _fov;

    private float _chaseSpeed;
    private bool _waiting;
    private float _waitTimer;
    public ChaseState(FSMEnemy fsm, Enemy enemy, float chaseSpeed)
    {
        _enemy = enemy;
        _fov = _enemy.Fov;
        this._fsm = fsm;
        this._chaseSpeed = chaseSpeed;
    }
    public override void OnEnter()
    {
        _waiting = false;
        _waitTimer = 0f;
        _fov.OnPlayerDetected += OnPlayerUnDetected;
        //_enemy.GetComponent<Renderer>().material.color = Color.red;
    }

    public override void OnExit()
    {
        _fov.OnPlayerDetected -= OnPlayerUnDetected;
        //_enemy.GetComponent<Renderer>().material.color = Color.magenta;
    }

    public override void OnUpdate()
    {
        if (_waiting)
        {
            _waitTimer += Time.deltaTime;

            if (_waitTimer >= 1f)
            {
                _fsm.ChangeState(EnemyStates.Patrol);
            }

            return;
        }

        _enemy.AddForce(_enemy.Seek(_fov.target.position), _chaseSpeed);
        _enemy.AddForce(_enemy.ObstacleAvoidance(), _chaseSpeed);
        _enemy.Move();
    }

    private void OnPlayerUnDetected(bool isDetected)
    {
        _waiting = !isDetected;
        _waitTimer = 0f;
    }

}
