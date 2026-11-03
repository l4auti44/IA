using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue
{
    Dictionary<Node, float> _allNodes = new Dictionary<Node, float>();

    public int Count { get { return _allNodes.Count; } }

    public void Put(Node node, float cost)
    {
        if (!_allNodes.ContainsKey(node))
            _allNodes.Add(node, cost);
        else 
            _allNodes[node] = cost;
    }

    public Node Get()
    {
        Node node = null;
        float lowestCost = Mathf.Infinity;

        foreach(var n in _allNodes)
        {
            if(n.Value < lowestCost)
            {
                lowestCost = n.Value;
                node = n.Key;
            }
        }
        _allNodes.Remove(node);
        return node;
    }

}
