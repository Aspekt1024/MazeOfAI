using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitPathfinder : MonoBehaviour {

    private float turnDist = 0.5f;
    private float stoppingDist = 1f;

    private Unit unit;
    private Path path;
    private Coroutine followRoutine = null;
    private Coroutine updateRoutine = null;
    private WorldGridManager worldGrid;
    private Section currentSection;
    private bool requirePathUpdate;

    private const float pathUpdateMoveThreshold = 0.7f;
    private const float minPathUpdateTime = 0.25f;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        worldGrid = GameObject.FindGameObjectWithTag("Scripts").GetComponent<WorldGridManager>();
    }

    private void Update()
    {
        Section section = worldGrid.GetSectionFromWorldPoint(transform.position);
        if (section != null && section != currentSection)
        {
            requirePathUpdate = true;
            currentSection = section;
        }
    }

    public void FindPath()
    {
        Stop();
        updateRoutine = StartCoroutine(UpdatePath());
    }

    public void Stop()
    {
        if (followRoutine != null) StopCoroutine(followRoutine);
        if (updateRoutine != null) StopCoroutine(updateRoutine);
    }
    
    private IEnumerator UpdatePath()
    {
        float squareMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetOldPos = unit.Target.position;
        PathRequestManager.RequestPath(new PathRequest(transform.position, unit.Target.position, gameObject, OnPathFound));

        float timeSinceUpdate = 0f;
        while (true)
        {
            if (Time.timeSinceLevelLoad < 2f) yield return new WaitForSeconds(2f);
            yield return new WaitForSeconds(minPathUpdateTime);

            if (unit.Target == null) continue;
            timeSinceUpdate += Time.deltaTime;
            if ((unit.Target.position - targetOldPos).sqrMagnitude > squareMoveThreshold || timeSinceUpdate > 0.5f || requirePathUpdate)
            {
                requirePathUpdate = false;
                PathRequestManager.RequestPath(new PathRequest(transform.position, unit.Target.position, gameObject, OnPathFound));
                targetOldPos = unit.Target.position;
            }
        }
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (unit == null) return;
        if (pathSuccessful)
        {
            path = new Path(waypoints, transform.position, turnDist, stoppingDist);
            if (followRoutine != null)  StopCoroutine(followRoutine);
            followRoutine = StartCoroutine(FollowPath());
        }
        else if (unit.Target != null && Vector2.Distance(Helpers.V3ToV2(transform.position), Helpers.V3ToV2(unit.Target.position)) < stoppingDist)
        {
            Stop();
            unit.TargetReached();
        }
        else
        {
            unit.FindAnotherPath();
        }
    }

    public void ForceTargetReached()
    {
        Stop();
        unit.TargetReached();
    }
    
    private IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        float speedPercent = 1f;
        
        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                speedPercent = 0;
                if (pathIndex >= path.slowDownIndex && stoppingDist > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDist);
                    if (float.IsNaN(speedPercent)) speedPercent = 0;
                    if (unit.Target == null)
                    {
                        yield break;
                    }
                    if (speedPercent < 0.01f || Vector3.Distance(unit.Target.position, transform.position) < 1f)  // TODO check for line of sight
                        followingPath = false;
                }

                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(path.lookPoints[pathIndex].x, 0, path.lookPoints[pathIndex].z) - new Vector3(pos2D.x, 0, pos2D.y));
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * unit.GetTurnSpeed());
                transform.rotation = Quaternion.LookRotation(new Vector3(transform.forward.x, -0.4f * speedPercent, transform.forward.z));
                Debug.Log(transform.forward.x + " " + transform.forward.z + " " + unit.GetSpeed() + " " + speedPercent);
                transform.position += new Vector3(transform.forward.x, 0, transform.forward.z) * Time.deltaTime * unit.GetSpeed() * speedPercent;
            }

            yield return null;
        }
        transform.rotation = Quaternion.LookRotation(new Vector3(transform.forward.x, 0f, transform.forward.z));
        unit.TargetReached();
    }
    
    private void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }
}
