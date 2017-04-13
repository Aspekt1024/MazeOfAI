using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Selectable {
    
    public float speed = 15f;
    public float turnDist = 0.5f;
    public float turnSpeed = 3f;
    public float stoppingDist = 2.5f;

    public string Name = "unnamed unit";
    public UnitButtons.WorkerTasks Task;

    protected float elevation;
    
    private Level level;
    private Transform player;

    private Transform target;
    private Path path;

    private Transform cargo;
    private Collider intersectingTargetCollider;

    private const float pathUpdateMoveThreshold = 0.7f;
    private const float minPathUpdateTime = 0.25f;

    // TODO set this per unit
    private enum NPCStates
    {
        Idle, Gathering, Building, Returning, Spawning
    }
    private NPCStates state;


    private void Awake()
    {
        GameObject scriptsObj = GameObject.FindGameObjectWithTag("Scripts");
        level = scriptsObj.GetComponent<Level>();

        player = GameObject.Find("Player").transform;
        state = NPCStates.Spawning;

        ObjRadius = 25f;
        SetupAttributes();
    }

    protected virtual void SetupAttributes()
    {
        Name = "unnamed unit";
        elevation = 0f;
        Task = UnitButtons.WorkerTasks.Idle;
    }

    private void Update()
    {
        if (state == NPCStates.Building && cargo != null)
        {
            cargo.transform.position = transform.position + transform.forward * 0.3f;
        }

        if (state != NPCStates.Idle || Task != UnitButtons.WorkerTasks.Gather) return;

        FindResourceToGather();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cargo == null || other.transform.parent != target || state != NPCStates.Building) return;
        intersectingTargetCollider = other;
        DepositCargo();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == intersectingTargetCollider)
            intersectingTargetCollider = null;
    }

    public void SetTask(UnitButtons.WorkerTasks newTask)
    {
        if (Task == newTask) return;
        Task = newTask;
    }

    private void FindResourceToGather()
    {
        GameObject[] resources = GameObject.FindGameObjectsWithTag("Resource");
        if (resources.Length == 0) return;

        for (int i = 0; i < resources.Length; i++)
        {
            if (!resources[i].GetComponent<Capsule>().Assigned && resources[i].GetComponent<Capsule>().Landed)
            {
                target = resources[i].transform;
                target.GetComponent<Capsule>().Assigned = true;
                state = NPCStates.Gathering;
                
                StartCoroutine(UpdatePath());
                break;
            }
        }
    }

    // TODO remove this and call it when we want. Need to revisit the state machine because it's not getting set exactly how we want
    private IEnumerator UpdatePath()
    {
        float squareMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetOldPos = target.position;
        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, gameObject, OnPathFound));

        float timeSinceUpdate = 0f;
        while (state != NPCStates.Idle)
        {
            if (Time.timeSinceLevelLoad < 2f) yield return new WaitForSeconds(2f);
            yield return new WaitForSeconds(minPathUpdateTime);
            if (target == null) continue;
            timeSinceUpdate += Time.deltaTime;
            if ((target.position - targetOldPos).sqrMagnitude > squareMoveThreshold || timeSinceUpdate > 0.5f)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, gameObject, OnPathFound));
                targetOldPos = target.position;
            }
        }
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Path(waypoints, transform.position, turnDist, stoppingDist);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
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
                if (pathIndex >= path.slowDownIndex && stoppingDist > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDist);
                    if (speedPercent < 0.01f || Vector3.Distance(target.position, transform.position) < 1.5f)
                        followingPath = false;
                }
                
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(path.lookPoints[pathIndex].x, 0, path.lookPoints[pathIndex].z) - new Vector3(pos2D.x, 0, pos2D.y));
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.rotation = Quaternion.LookRotation(new Vector3(transform.forward.x, -0.4f * speedPercent, transform.forward.z));
                transform.position += new Vector3(transform.forward.x, 0, transform.forward.z) * Time.deltaTime * speed * speedPercent;
                
                if (state == NPCStates.Building && intersectingTargetCollider != null && intersectingTargetCollider.transform.parent == target)
                {
                    break;
                }

                if (Task == UnitButtons.WorkerTasks.Idle)
                {
                    if (target != null && target.GetComponent<Capsule>() != null)
                    {
                        target.GetComponent<Capsule>().Assigned = false;
                        target = null;
                    }
                    state = NPCStates.Idle;
                }

                if (state == NPCStates.Idle)
                    yield break;
            }

            yield return null;
        }
        transform.rotation = Quaternion.LookRotation(new Vector3(transform.forward.x, 0f, transform.forward.z));
        TargetReached();
    }

    private void TargetReached()
    {
        switch(state)
        {
            case NPCStates.Gathering:
                state = NPCStates.Building;
                cargo = target;
                StopCoroutine(FollowPath());
                target = FindObjectOfType<Building>().transform;
                break;
            case NPCStates.Building:
                DepositCargo();
                break;
        }
    }

    private void DepositCargo()
    {
        StopCoroutine(FollowPath());
        StopCoroutine(UpdatePath());
        state = NPCStates.Idle;
        Destroy(cargo.gameObject);
        target.GetComponent<Building>().IncreaseCompletion(0.1f);
        GameData.Instance.Capsules += 1;
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }

    public void MoveUnitToPoint(Vector3 startPoint, Vector3 endPoint)
    {
        StartCoroutine(MoveToPoint(startPoint, endPoint));
    }

    private IEnumerator MoveToPoint(Vector3 startPoint, Vector3 endPoint)
    {
        transform.position = new Vector3(startPoint.x, elevation, startPoint.z);
        while (Vector2.Distance(V3ToV2(transform.position), V3ToV2(endPoint)) > .1f)
        {
            transform.LookAt(new Vector3(endPoint.x, elevation, endPoint.z));
            transform.position += new Vector3(transform.forward.x, 0, transform.forward.z) * Time.deltaTime * speed;
            yield return null;
        }
        state = NPCStates.Idle;
    }

    private static Vector2 V3ToV2(Vector3 V3)
    {
        return new Vector2(V3.x, V3.z);
    }
}
