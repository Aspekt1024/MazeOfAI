using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : IHeapItem<PathNode> {

    public bool walkable;
    public Vector3 worldPosition;

    public int gCost;
    public int hCost;

    public int gridX;
    public int gridY;

    public PathNode parent;

    private int heapIndex;

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

    public int HeapIndex
    {
        get {
            return heapIndex;
        }
        set { 
            heapIndex = value;
        }
    }

    public int CompareTo(PathNode nodeToCompate)
    {
        int compare = fCost.CompareTo(nodeToCompate.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompate.hCost);
        }
        return -compare;
    }

}
