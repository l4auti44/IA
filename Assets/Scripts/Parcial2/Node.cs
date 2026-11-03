using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Node : MonoBehaviour, IPointerClickHandler
{
    [Header("Up, Down, Left, Right")]
    [SerializeField] List<Node> _neighbors = new List<Node>();
    Grid _grid;
    public bool isBlock = false;
    public int cost = 1;
    [SerializeField] TextMeshProUGUI textCost;
   

    public void Initialize(int x, int y, Grid g)
    {
        //_x = x;
        //_y = y;
        _grid = g;
        SetCost(0);
    }

    public List<Node> GetNeighbors()
    {
        /*
        if(_neighbors.Count > 0) return _neighbors;

        Node newNeighbor = _grid.GetNode(_x, _y + 1); //Up
        if(newNeighbor != null) _neighbors.Add(newNeighbor);

        newNeighbor = _grid.GetNode(_x, _y - 1); //Down
        if (newNeighbor != null) _neighbors.Add(newNeighbor);

        newNeighbor = _grid.GetNode(_x + 1, _y); // Right
        if (newNeighbor != null) _neighbors.Add(newNeighbor);

        newNeighbor = _grid.GetNode(_x - 1, _y); // Left
        if (newNeighbor != null) _neighbors.Add(newNeighbor);

        return _neighbors;
        */
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var neighbor in _neighbors) 
        {
            if (neighbor != null)
            {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (Keyboard.current.ctrlKey.isPressed)
            {
                SetCost(5);
            }
            else
            {
                //GameManager.instance.SetStartingNode(this);
            }
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            if (Keyboard.current.ctrlKey.isPressed)
            {
                SetCost(-5);
            }
            else
            {
                //GameManager.instance.SetEndNode(this);
            }
        }
        else if(eventData.button == PointerEventData.InputButton.Middle)
        {
            isBlock = !isBlock;
            //GameManager.instance.PaintGameObject(gameObject, isBlock ? Color.gray : Color.white);
        }
    }

    public void SetCost(int amount)
    {
        cost += amount;
        cost = Mathf.Clamp(cost, 1 , 99);

        textCost.text = cost.ToString();
        textCost.enabled = cost != 1;
    }
}
