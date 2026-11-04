using System;
using UnityEngine;

public readonly struct PlayerFoundData
{

    public readonly Node playerClosestNode;

    public PlayerFoundData(Node playerClosestNode)
    {
        this.playerClosestNode = playerClosestNode;
    }


}