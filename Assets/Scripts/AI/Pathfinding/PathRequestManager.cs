using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PathRequestManager : MonoBehaviour {
    
    private List<PathRequest> requests = new List<PathRequest>();
    private static PathRequestManager instance;
    private Pathfinder pathFinder;

    private void Awake()
    {
        instance = this;
        pathFinder = gameObject.AddComponent<Pathfinder>();
    }

    private void Update()
    {
        if (requests.Count == 0 || pathFinder.IsProcessing()) return;

        PathRequest request = requests[0];
        requests.Remove(request);
        instance.pathFinder.FindPath(request, FinishedProcessingPath);
    }

    public static void RequestPath(PathRequest request)
    {
        Queue<PathRequest> pendingRequestsSameCaller = new Queue<PathRequest>();
        foreach (var req in instance.requests)
        {
            if (req.caller == request.caller)
                pendingRequestsSameCaller.Enqueue(req);
        }
        while (pendingRequestsSameCaller.Count > 0)
        {
            instance.requests.Remove(pendingRequestsSameCaller.Dequeue());
        }

        instance.requests.Add(request);
    }

    public void FinishedProcessingPath(PathResult result)
    {
        result.callback(result.path, result.success);
    }

}

public struct PathResult
{
    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool> callback;

    public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
    {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}

public struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public GameObject caller;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 start, Vector3 end, GameObject callingObj, Action<Vector3[], bool> cb)
    {
        pathStart = start;
        pathEnd = end;
        caller = callingObj;
        callback = cb;
    }
}