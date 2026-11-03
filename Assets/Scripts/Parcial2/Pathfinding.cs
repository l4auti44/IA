using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{

    public float HeuristicManhattan(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public List<Node> AStar(Node start, Node end)
    {

        
        if (start == null || end == null) return null;

        PriorityQueue frontier = new PriorityQueue();
        frontier.Put(start, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(start, null);

        Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();
        costSoFar.Add(start, 0);

        while (frontier.Count > 0)
        {
            Node current = frontier.Get();
            //GameManager.instance.PaintGameObject(current.gameObject, Color.green);
            //yield return new WaitForSeconds(0.1f);

            if (current == end)
            {
                List<Node> path = new List<Node>();

                Node prev = current;
                while (prev != null)
                {
                    path.Add(prev);
                    prev = cameFrom[prev];
                }

                foreach (Node node in path)
                {
                    //GameManager.instance.PaintGameObject(node.gameObject, Color.magenta);
                }
                path.Reverse();
                return path;
            }

            foreach (Node next in current.GetNeighbors())
            {
                if (next == null || next.isBlock) continue;

                float heuristic = (end.transform.position - next.transform.position).magnitude;
                float newCost = costSoFar[current] + next.cost;
                float priority = heuristic + newCost;

                if (!cameFrom.ContainsKey(next))
                {
                    frontier.Put(next, priority);
                    cameFrom.Add(next, current);
                    costSoFar.Add(next, newCost);

                    // GameManager.instance.PaintGameObject(next.gameObject, Color.blue);
                }
                else if (costSoFar[next] > newCost)
                {
                    frontier.Put(next, priority);
                    cameFrom[next] = current;
                    costSoFar[next] = newCost;
                }
            }

        }
        return null;
    }
}
