using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {
    
    private PathGrid grid;
    private PathRequestManager requestManager;

    private void Awake()
    {
        GameObject scriptsObj = GameObject.FindGameObjectWithTag("Scripts");
        grid = scriptsObj.GetComponent<PathGrid>();
        requestManager = scriptsObj.GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }
    
    private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathFound = false;

        PathNode startNode = grid.GetNodeFromWorldPoint(startPos);
        PathNode targetNode = grid.GetNodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<PathNode> openSet = new Heap<PathNode>(grid.MaxSize);
            HashSet<PathNode> closedSet = new HashSet<PathNode>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                PathNode currentNode = openSet.RemoveFirst();
                /* This is kept for educational reference. The Heap<T> class replaces the need for this unoptimised code
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                        currentNode = openSet[i];
                }
                openSet.Remove(currentNode);*/

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathFound = true;
                    break;
                }
                foreach (PathNode neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                    int newMoveCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMoveCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMoveCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        yield return null;

        if (pathFound)
            waypoints = RetracePath(startNode, targetNode);

        requestManager.FinishedProcessingPath(waypoints, pathFound);
    }

    private int GetDistance(PathNode nodeA, PathNode nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        else
            return 14 * distX + 10 * (distY - distX);
    }

    private Vector3[] RetracePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(path);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<PathNode> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i-1].worldPosition);
                directionOld = directionNew;
            }
        }
        waypoints.Reverse();
        return waypoints.ToArray();
    }
}
