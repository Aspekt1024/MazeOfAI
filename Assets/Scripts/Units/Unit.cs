using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public abstract class Unit : Selectable {

    public Transform Target;

    protected bool UnitActive;
    protected float elevation;
    protected UnitPathfinder pathfinder;
    protected Experience xpHandler;

    protected float baseSpeed = 5f;
    protected float speedMultiplier = 1f;
    protected float baseTurnSpeed = 3f;
    protected float turnSpeedMultiplier = 1f;

    public abstract void TargetReached();

    private void Awake()
    {
        pathfinder = gameObject.AddComponent<UnitPathfinder>();
        xpHandler = gameObject.AddComponent<Experience>();
        
        ObjRadius = 25f;
        SetupAttributes();
    }
    
    protected virtual void SetupAttributes()
    {
        Name = "unnamed unit";
        elevation = 0f;
    }
    
    public float GetSpeed()
    {
        return baseSpeed * speedMultiplier;
    }

    public float GetTurnSpeed()
    {
        return baseTurnSpeed * turnSpeedMultiplier;
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
    public virtual void UpdateAttributesForLevel(int level) { }
}
