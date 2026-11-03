using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
    [SerializeField] protected float _maxSpeed, _maxForce;
    [SerializeField] protected float _viewRadius, _separationRadius;
    [SerializeField] LayerMask _obstacles;
    protected Vector3 _velocity;
    [SerializeField, Min(0f)] float _evadePredictionTime = 1f;
    [SerializeField, Min(0.01f)] float _wanderInterval = 1f;
    [SerializeField, Range(0f, 180f)] float _wanderTurnAngle = 60f;
    private Vector3 _wanderDirection;
    private float _nextWanderTime;

    public bool DEBUG = false;

    protected void Move()
    {
        _velocity = new Vector3(_velocity.x, _velocity.y, 0);  //FORCE 2D
        transform.position += _velocity * Time.deltaTime;
        if (_velocity.sqrMagnitude > 0.000001f)
            transform.right = _velocity;
    }

    public Vector3 Seek(Vector3 target)
    {
    
        return Seek(target, _maxSpeed);
    }

    public Vector3 Seek(Vector3 target, float speed)
    {
        Vector3 desired = target - transform.position;
        desired.Normalize();
        desired *= speed;
        Debug.DrawLine(transform.position, transform.position + desired, Color.gray);

        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce * Time.deltaTime);

        return steering;
    }

    public Vector3 Flee(Vector3 target)
    {
        // -Seek(target)
        Vector3 direction = transform.position - target;
        direction.z = 0f;
        return CalculateSteering(direction.normalized * _maxSpeed);
    }

    public Vector3 Pursuit(SteeringBehaviors targetAgent)
    {
        Vector3 futurePos = targetAgent.transform.position + targetAgent._velocity;
        Debug.DrawLine(transform.position, futurePos, Color.magenta);
        return Seek(futurePos);
    }

    public Vector3 Evade(SteeringBehaviors targetAgent)
    {
        return Evade(targetAgent.transform.position, targetAgent._velocity);
    }

    public Vector3 Evade(Vector3 targetPosition, Vector3 targetVelocity)
    {
        // -Pursuit(target)
        float distance = Vector3.Distance(transform.position, targetPosition);
        float predictionTime = Mathf.Min(_evadePredictionTime, distance / Mathf.Max(_maxSpeed, 0.01f));
        Vector3 futurePosition = targetPosition + targetVelocity * predictionTime;
        return Flee(futurePosition);
    }

    public Vector3 Wander()
    {
        if (Time.time >= _nextWanderTime || _wanderDirection == Vector3.zero)
        {
            Vector3 heading = _velocity.sqrMagnitude > 0.000001f ? _velocity.normalized : transform.right;
            float angle = Random.Range(-_wanderTurnAngle, _wanderTurnAngle);
            _wanderDirection = Quaternion.AngleAxis(angle, Vector3.forward) * heading;
            _nextWanderTime = Time.time + _wanderInterval;
        }

        return CalculateSteering(_wanderDirection * _maxSpeed);
    }

    public Vector3 Arrive(Vector3 target)
    {
        float dist = Vector3.Distance(transform.position, target);
        if (_viewRadius <= 0f || dist >= _viewRadius) return Seek(target);

        return Seek(target, _maxSpeed * (dist / _viewRadius));
    }

    public Vector3 ObstacleAvoidance()
    {
        TryGetObstacleAvoidance(out Vector3 steering);
        return steering;
    }

    public bool TryGetObstacleAvoidance(out Vector3 steering)
    {
        steering = Vector3.zero;
        if (DEBUG)
        {
            Debug.DrawLine(transform.position + transform.up * 0.7f, transform.position + transform.up * 0.7f + transform.right * _viewRadius);
            Debug.DrawLine(transform.position - transform.up * 0.7f, transform.position - transform.up * 0.7f + transform.right * _viewRadius);
        }


        if (Physics.Raycast(transform.position + transform.up * 0.7f, transform.right, _viewRadius, _obstacles))
        {
            steering = Seek(transform.position - transform.up);
            return true;
        }
        else if (Physics.Raycast(transform.position - transform.up * 0.7f, transform.right, _viewRadius, _obstacles))
        {
            steering = Seek(transform.position + transform.up);
            return true;
        }

        return false;
    }

    public bool HasToUseObstacleAvoidance()
    {
        return TryGetObstacleAvoidance(out _);
    }

    public Vector3 Alignment(List<SteeringBehaviors> agents)
    {
        Vector3 desired = Vector3.zero;
        int boidsCount = 0;

        foreach (SteeringBehaviors item in agents)
        {
            if (item == null || item == this) continue;
            if ((item.transform.position - transform.position).sqrMagnitude > _viewRadius * _viewRadius) continue;

            desired += item._velocity;
            boidsCount++;
        }

        if (boidsCount == 0) return Vector3.zero;

        desired /= boidsCount;

        return CalculateSteering(desired.normalized * _maxSpeed);
    }

    public Vector3 CalculateSteering(Vector3 desired)
    {
        return Vector3.ClampMagnitude(desired - _velocity, _maxForce * Time.deltaTime);
    }

    public void AddForce(Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, _maxSpeed);
    }

    protected virtual void OnDrawGizmos()
    {
        if (!DEBUG) return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + _velocity);
        Gizmos.DrawWireSphere(transform.position, _viewRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _separationRadius);

    }

    public Vector3 Cohesion(List<SteeringBehaviors> agents)
    {
        Vector3 desired = Vector3.zero;
        int boidsCount = 0;

        foreach (SteeringBehaviors item in agents)
        {
            if (item == null || item == this) continue;
            Vector3 dist = item.transform.position - transform.position;
            if (dist.sqrMagnitude > _viewRadius * _viewRadius) continue;

            desired += item.transform.position;
            boidsCount++;
        }

        if (boidsCount == 0) return Vector3.zero;

        desired /= boidsCount;
        return Seek(desired);
    }

    public Vector3 Separation(List<SteeringBehaviors> agents)
    {
        Vector3 desired = Vector3.zero;

        foreach (SteeringBehaviors item in agents)
        {
            if (item == null || item == this) continue;

            Vector3 dist = item.transform.position - transform.position;
            if (dist.sqrMagnitude > _separationRadius * _separationRadius) continue;

            desired += dist;
        }

        if (desired == Vector3.zero) return desired;

        desired *= -1;

        return CalculateSteering(desired.normalized * _maxSpeed);
    }


    //https://www.red3d.com/cwr/steer/gdc99/
}
