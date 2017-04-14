using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public struct Task
{
    public string taskName;
    public int taskId;

    public Task(string task, int id)
    {
        taskName = task;
        taskId = id;
    }
}

public abstract class Unit : Selectable {

    public float baseSpeed = 5f;
    public float speedMultiplier = 1f;
    public string Name = "unnamed unit";

    public Transform Target;

    protected bool UnitActive;
    protected float elevation;
    protected UnitPathfinder pathfinder;
    
    private void Awake()
    {
        pathfinder = gameObject.AddComponent<UnitPathfinder>();
        
        ObjRadius = 25f;
        SetupAttributes();
    }

    protected virtual void SetupAttributes()
    {
        Name = "unnamed unit";
        elevation = 0f;
    }
    
    public virtual List<Task> GetTaskList()
    {
        return new List<Task>();
    }

    public virtual void SetTask(int taskId) { }
    
    public abstract void TargetReached();
    
    public float GetSpeed()
    {
        return baseSpeed * speedMultiplier;
    }

    public void MoveUnitToPoint(Vector3 startPoint, Vector3 endPoint)
    {
        StartCoroutine(MoveToSpawnRally(startPoint, endPoint));
    }

    private IEnumerator MoveToSpawnRally(Vector3 startPoint, Vector3 endPoint)
    {
        transform.position = new Vector3(startPoint.x, elevation, startPoint.z);
        while (Vector2.Distance(Helpers.V3ToV2(transform.position), Helpers.V3ToV2(endPoint)) > .1f)
        {
            transform.LookAt(new Vector3(endPoint.x, elevation, endPoint.z));
            transform.position += new Vector3(transform.forward.x, 0, transform.forward.z) * Time.deltaTime * GetSpeed();
            yield return null;
        }
        UnitActive = true;
    }

    public abstract void FindAnotherPath();
}
