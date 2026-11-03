using System.Collections.Generic;
using UnityEngine;

public class Hunter : SteeringBehaviors
{
    public float energy = 10f;
    private float originalEnergy;
    [SerializeField] private float restTime = 5f;
    [SerializeField] private List<Transform> patrolPositions = new List<Transform>();
    FiniteStateMachine _fsm;
    private Vector3 nextWaypoint = Vector3.zero;
    private int currentIndexWaypoint = 0;

    public SteeringBehaviors targetBoid { get; private set; }
    void Start()
    {
        _fsm = new FiniteStateMachine();
        originalEnergy = energy;
        GameManager.instance.allHunters.Add(this);

        patrolPositions.RemoveAll(waypoint => waypoint == null);
        if (patrolPositions.Count > 0)
        {
            nextWaypoint = patrolPositions[0].position;
            _velocity = (nextWaypoint - transform.position).normalized * _maxSpeed;
        }

        _fsm.AddStates(PlayerStates.Idle, new IdleState(_fsm, this, restTime));
        _fsm.AddStates(PlayerStates.Patrol, new PatrolState(_fsm, this));
        _fsm.AddStates(PlayerStates.Hunt, new HuntState(_fsm, this));

        _fsm.ChangeState(PlayerStates.Patrol);
    }

    void Update()
    {
        _fsm.Update();
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null)
            GameManager.instance.allHunters.Remove(this);
    }

    public void OnPatrol()
    {
        if (patrolPositions.Count == 0)
        {
            StopMovement();
            return;
        }

        if (TryAvoidObstacleMovement()) return;

        MoveToNextWaypoint();
    }

    private void MoveToNextWaypoint()
    {
        AddForce(Seek(nextWaypoint));
        if (Vector3.Distance(transform.position, nextWaypoint) <= 0.3f)
        {
            nextWaypoint = GetNextWaypoint();
        }
        CompleteMovement();
    }

    private Vector3 GetNextWaypoint()
    {
        if (currentIndexWaypoint + 1 >= patrolPositions.Count) currentIndexWaypoint = 0;

        else currentIndexWaypoint++;

        return patrolPositions[currentIndexWaypoint].position;

    }

    public void OnHunt()
    {
        GetClosestBoid();
        if (targetBoid == null) return;
        if (TryAvoidObstacleMovement()) return;

        AddForce(Pursuit(targetBoid));
        if ((targetBoid.transform.position - transform.position).sqrMagnitude < 0.5f)
        {
            GameManager.instance.allAgents.Remove(targetBoid);
            Destroy(targetBoid.gameObject);
            ClearTarget();
        }
        CompleteMovement();
    }

    private void GetClosestBoid()
    {
        targetBoid = null;
        float nearestDistanceSquared = _viewRadius * _viewRadius;
        foreach (SteeringBehaviors agent in GameManager.instance.allAgents)
        {
            if (agent == null || !agent.isActiveAndEnabled || !(agent is Boid)) continue;
            float distanceSquared = (agent.transform.position - transform.position).sqrMagnitude;
            if (distanceSquared >= nearestDistanceSquared) continue;

            nearestDistanceSquared = distanceSquared;
            targetBoid = agent;
        }
    }

    public bool HasNearbyBoids()
    {
        GetClosestBoid();
        return targetBoid != null;
    }

    public void ClearTarget()
    {
        targetBoid = null;
    }

    public void StopMovement()
    {
        _velocity = Vector3.zero;
    }

    public void ResetEnergy()
    {
        energy = originalEnergy;
    }

    private bool TryAvoidObstacleMovement()
    {
        if (!TryGetObstacleAvoidance(out Vector3 avoidance)) return false;

        AddForce(avoidance);
        CompleteMovement();
        return true;
    }

    private void CompleteMovement()
    {
        Move();
        transform.position = GameManager.instance.AdjustPositionToBounds(transform.position);
    }

}
