using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {
    
    private PathGrid grid;
    private bool findingPath;

    private void Awake()
    {
        GameObject scriptsObj = GameObject.FindGameObjectWithTag("Scripts");
        grid = scriptsObj.GetComponent<PathGrid>();
    }

    // TODO handle generation of grid (currently in NPC)
    
    public void FindPath(PathRequest request, Action<PathResult> callback)
    {
        StopCoroutine("PathSearch");
        StartCoroutine(PathSearch(request, callback));
    }

    private IEnumerator PathSearch(PathRequest request, Action<PathResult> callback)
    {
        Vector3[] waypoints = new Vector3[0];
        PathNode targetNode = grid.GetNearestWalkableNode(request.pathEnd);
        PathNode startNode = grid.GetNearestWalkableNode(request.pathStart);
        bool pathFound = false;

        findingPath = true;

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<PathNode> openSet = new Heap<PathNode>(grid.MaxSize);
            HashSet<PathNode> closedSet = new HashSet<PathNode>();
            openSet.Add(startNode);

            yield return null;

            int iter = 0;
            while (openSet.Count > 0)
            {
                iter++;
                if (iter > 1000)
                {
                    iter = 0;
                    yield return null;
                }
                PathNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);
                
                if (currentNode == targetNode)
                {
                    pathFound = true;
                    break;
                }

                foreach (PathNode neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                    int newMoveCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + currentNode.movementPenalty;
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

        if (pathFound)
        {
            waypoints = RetracePath(startNode, targetNode);
            pathFound = waypoints.Length > 0;
        }

        callback(new PathResult(waypoints, pathFound, request.callback));
        yield return null;
        findingPath = false;
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

    private Vector3[] SimplifyPath(List<PathNode> path)
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

    public bool IsProcessing()
    {
        return findingPath;
    }
}
