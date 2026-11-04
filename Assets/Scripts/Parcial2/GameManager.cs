using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<Node> nodes = new List<Node>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void FindNearestNode(Vector3 position, out Node nearestNode)
    {
        nearestNode = null;
        var currentDistance = float.MaxValue;
        foreach (Node node in Instance.nodes)
        {
            float distance = Vector3.Distance(position, node.transform.position);
            if (distance < currentDistance)
            {
                currentDistance = distance;
                nearestNode = node;
            }
        }
    }


}
