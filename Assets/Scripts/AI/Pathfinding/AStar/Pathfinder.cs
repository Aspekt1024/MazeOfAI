using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {
    
    private PathGrid grid;
    private PathGridSection startSection;
    private PathGridSection endSection;
    private bool processingPath;
    private bool gridGenRequested;

    private Coroutine pathSearchCoroutine = null;
    private Coroutine pathSearchProcess = null;
    private PathRequest request;
    private Action<PathResult> callback;

    private bool pathFound;
    private Vector3[] waypoints;

    private WorldGridManager worldGrid;

    private void Start()
    {
        worldGrid = GetComponent<WorldGridManager>();
    }
    
    private void Update()
    {
        if (Time.timeSinceLevelLoad < 2f || gridGenRequested) return;
        gridGenRequested = true;
    }

    public void FindPath(PathRequest req, Action<PathResult> cb)
    {
        processingPath = true;
        request = req;
        callback = cb;

        startSection = worldGrid.GetSectionFromWorldPoint(request.pathStart);
        endSection = worldGrid.GetSectionFromWorldPoint(request.pathEnd);

        if (pathSearchProcess != null)
            StopCoroutine(SearchProcess());

        pathSearchProcess = StartCoroutine(SearchProcess());
    }

    private IEnumerator SearchProcess()
    {
        if (pathSearchCoroutine != null)
            StopCoroutine(pathSearchCoroutine);

        if (startSection == null || endSection == null)
        {
            processingPath = false;
            callback(new PathResult(new Vector3[0], false, request.callback));
        }

        if (startSection == endSection)
        {
            grid = startSection.grid;
            yield return pathSearchCoroutine = StartCoroutine(PathSearch());
            callback(new PathResult(waypoints, pathFound, request.callback));
            processingPath = false;
        }
        else
        {


            processingPath = false;
            callback(new PathResult(new Vector3[0], false, request.callback));


            //// see if any points in the next section are in line of sight
            //Vector3 newEndpoint = request.pathEnd;

            //Vector3 pathEnd = request.pathEnd;
            ////request.pathEnd = endSection.grid.EntryPoint;
            //endSection = worldGrid.GetSectionFromWorldPoint(request.pathEnd);
            //grid = startSection.grid;
            //pathSearchCoroutine = StartCoroutine(PathSearch());

            //AddEndNodeToWaypoints(pathEnd);
            //callback(new PathResult(waypoints, pathFound, request.callback));
            //processingPath = false;
        }

    }

    private IEnumerator PathSearch()
    {
        while (!grid.GridGenerated)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        PathNode targetNode = grid.GetNearestWalkableNode(request.pathEnd);
        PathNode startNode = grid.GetNearestWalkableNode(request.pathStart);
        waypoints = new Vector3[0];
        pathFound = false;
        
        if (startNode.walkable && targetNode.walkable)
        {
            Heap<PathNode> openSet = new Heap<PathNode>(grid.MaxSize);
            HashSet<PathNode> closedSet = new HashSet<PathNode>();
            openSet.Add(startNode);

            float timer = Time.realtimeSinceStartup;
            float maxTime = .01f;
            while (openSet.Count > 0)
            {
                if (Time.realtimeSinceStartup > timer + maxTime)
                {
                    yield return null;
                    timer = Time.realtimeSinceStartup;
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
    }

    private void AddEndNodeToWaypoints(Vector3 endNode)
    {
        Vector3[] newWaypoints = new Vector3[waypoints.Length + 1];
        for (int i = 0; i < waypoints.Length; i++)
        {
            newWaypoints[i] = waypoints[i];
        }
        newWaypoints[waypoints.Length] = endNode;
        waypoints = newWaypoints;
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
        return processingPath;
    }
}
