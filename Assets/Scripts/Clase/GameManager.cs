using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] float _boundWidth, _boundHeight;

    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private float foodSpawnInterval = 5f;
    [SerializeField, Min(0.01f)] private float foodSpawnRadius = 0.6f;
    [SerializeField, Min(1)] private int foodSpawnAttempts = 20;
    [SerializeField] private LayerMask foodObstacleMask;

    public List<SteeringBehaviors> allAgents = new List<SteeringBehaviors>();
    public List<Hunter> allHunters = new List<Hunter>();
    public List<Transform> allFood = new List<Transform>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    void Start()
    {
        SpawnFood();
        StartCoroutine(SpawnFoodRutine());
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(_boundWidth, _boundHeight));
    }

    public Vector3 AdjustPositionToBounds(Vector3 pos)
    {
        float width = _boundWidth / 2;
        float height = _boundHeight / 2;

        if (pos.y > height) pos.y = -height;
        if (pos.y < -height) pos.y = height;

        if (pos.x > width) pos.x = -width;
        if (pos.x < -width) pos.x = width;

        return pos;
    }

    private IEnumerator SpawnFoodRutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(foodSpawnInterval);
            SpawnFood();
        }

    }
    private void SpawnFood()
    {
        for (int attempt = 0; attempt < foodSpawnAttempts; attempt++)
        {
            Vector3 randomFoodPos = new Vector3(Random.Range(-_boundWidth / 2, _boundWidth / 2), Random.Range(-_boundHeight / 2, _boundHeight / 2), 0f);
            if (Physics.CheckSphere(randomFoodPos, foodSpawnRadius, foodObstacleMask, QueryTriggerInteraction.Collide))
                continue;

            var newFood = Instantiate(foodPrefab, randomFoodPos, Quaternion.identity);
            allFood.Add(newFood.transform);
            return;
        }

        // Si no hay espacio libre, se vuelve a intentar en el siguiente intervalo.
    }

    public void RemoveFood(Vector3 targetFood)
    {
        foreach (Transform food in allFood)
        {
            if (food.position == targetFood)
            {
                allFood.Remove(food);
                Destroy(food.gameObject);
                return;
            }
        }
    }


}
