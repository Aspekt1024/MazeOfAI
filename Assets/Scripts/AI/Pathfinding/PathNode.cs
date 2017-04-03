using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode {

    public bool walkable;
    public Vector3 worldPosition;

    public int gCost;
    public int hCost;

    public int gridX;
    public int gridY;

    public PathNode parent;

    public PathNode(bool canWalk, Vector3 worldPos, int xIndex, int yIndex)
    {
        walkable = canWalk;
        worldPosition = worldPos;
        gridX = xIndex;
        gridY = yIndex;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }

}
