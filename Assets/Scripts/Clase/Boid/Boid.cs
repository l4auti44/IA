using UnityEngine;

public class Boid : SteeringBehaviors
{
    [SerializeField, Range(0f, 2.5f)] float _alignmentWeight = 1;
    [SerializeField, Range(0f, 2.5f)] float _separationWeight = 1;
    [SerializeField, Range(0f, 2.5f)] float _cohesionWeight = 1;
    [SerializeField, Range(0f, 2.5f)] float _pickUpFoodOffset = 1;
    private float separationWeight;
    private Vector3 nearestFood;
    private Hunter nearestHunter;

    void Start()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);

        Vector3 dir = new Vector3(x, y);

        _velocity = dir.normalized * _maxSpeed;

        GameManager.instance.allAgents.Add(this);

        separationWeight = _separationWeight;
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null)
            GameManager.instance.allAgents.Remove(this);
    }

    public bool IsThreatInView()
    {
        nearestHunter = null;
        float nearestDistanceSquared = _viewRadius * _viewRadius;

        foreach (Hunter hunter in GameManager.instance.allHunters)
        {
            if (hunter == null || !hunter.isActiveAndEnabled) continue;

            float distanceSquared = (hunter.transform.position - transform.position).sqrMagnitude;
            if (distanceSquared >= nearestDistanceSquared) continue;

            nearestDistanceSquared = distanceSquared;
            nearestHunter = hunter;
        }

        return nearestHunter != null;
    }

    public bool HasNearbyBoids()
    {
        foreach (SteeringBehaviors agent in GameManager.instance.allAgents)
        {
            if (agent == null || agent == this || !(agent is Boid)) continue;
            if ((agent.transform.position - transform.position).sqrMagnitude < _viewRadius * _viewRadius)
                return true;
        }

        return false;
    }

    public void OnMovement()
    {
        if (TryAvoidObstacleMovement()) return;

        var boids = GameManager.instance.allAgents;


        AddForce(Alignment(boids) * _alignmentWeight);
        AddForce(Cohesion(boids) * _cohesionWeight);

        AddForce(Separation(boids) * _separationWeight);


        CompleteMovement();
    }

    public void SeekFood()
    {
        if (TryAvoidObstacleMovement()) return;

        var boids = GameManager.instance.allAgents;
        AddForce(Arrive(nearestFood));
        AddForce(Separation(boids) * _separationWeight);
        if (Vector3.Distance(transform.position, nearestFood) <= _pickUpFoodOffset) GameManager.instance.RemoveFood(nearestFood);
        CompleteMovement();

    }

    public void SetSeparationWeight(float value)
    {
        _separationWeight = value;
    }

    public void ResetSeparationWeight()
    {
        _separationWeight = separationWeight;
    }

    public bool FoodInRange()
    {
        bool found = false;
        float nearestDistanceSquared = _viewRadius * _viewRadius;

        foreach (Transform food in GameManager.instance.allFood)
        {
            if (food == null) continue;

            float distanceSquared = (food.position - transform.position).sqrMagnitude;
            if (distanceSquared >= nearestDistanceSquared) continue;

            nearestDistanceSquared = distanceSquared;
            nearestFood = food.position;
            found = true;
        }

        return found;
    }

    public void OnFlee()
    {
        if (nearestHunter == null) return;
        if (TryAvoidObstacleMovement()) return;

        var boids = GameManager.instance.allAgents;
        AddForce(Separation(boids) * _separationWeight);
        AddForce(Evade(nearestHunter));
        CompleteMovement();
    }

    public void OnWander()
    {
        if (TryAvoidObstacleMovement()) return;

        AddForce(Wander());
        CompleteMovement();
    }

    public void OnAvoiding()
    {
        if (!TryAvoidObstacleMovement()) CompleteMovement();
    }

    private bool TryAvoidObstacleMovement()
    {
        if (!TryGetObstacleAvoidance(out Vector3 avoidance)) return false;

        AddForce(avoidance);
        AddForce(Separation(GameManager.instance.allAgents) * 0.3f);
        CompleteMovement();
        return true;
    }

    private void CompleteMovement()
    {
        Move();
        transform.position = GameManager.instance.AdjustPositionToBounds(transform.position);
    }
}
