using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    
    public float speed = 15f;
    public float turnDist = 0.5f;
    public float turnSpeed = 3f;
    public float stoppingDist = 2.5f;

    public string Name = "unnamed unit";
    public UnitButtons.WorkerTasks Task;

    protected float elevation;

    private AIMovement movement;
    private Level level;
    private Transform player;

    private Transform target;
    private Path path;

    private Transform cargo;

    private const float pathUpdateMoveThreshold = 0.7f;
    private const float minPathUpdateTime = 0.25f;

    private enum NPCStates
    {
        Idle, Gathering, Building, Returning
    }
    private NPCStates state;


    private void Awake()
    {
        SetupAttributes();
        GameObject scriptsObj = GameObject.FindGameObjectWithTag("Scripts");
        level = scriptsObj.GetComponent<Level>();

        player = GameObject.Find("Player").transform;

        movement = gameObject.AddComponent<AIMovement>();
    }


    private void Start()
    {
        StartCoroutine(SetupNPC());
        Task = UnitButtons.WorkerTasks.Idle;
    }

    protected virtual void SetupAttributes()
    {
        Name = "unnamed unit";
        elevation = 0f;
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
            if (!resources[i].GetComponent<Capsule>().Assigned)
            {
                target = resources[i].transform;
                target.GetComponent<Capsule>().Assigned = true;
                state = NPCStates.Gathering;

                StartCoroutine(UpdatePath());
                break;
            }
        }
    }

    private IEnumerator SetupNPC()
    {
        const float timeBeforeStart = 1f;
        yield return new WaitForSeconds(timeBeforeStart);
    }

    private IEnumerator UpdatePath()
    {
        float squareMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetOldPos = target.position;
        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, gameObject, OnPathFound));

        while (true)
        {
            if (Time.timeSinceLevelLoad < 2f) yield return new WaitForSeconds(2f);
            yield return new WaitForSeconds(minPathUpdateTime);
            if (target == null) continue;
            if ((target.position - targetOldPos).sqrMagnitude > squareMoveThreshold)
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

                if (Task == UnitButtons.WorkerTasks.Idle)
                {
                    target.GetComponent<Capsule>().Assigned = false;
                    if (state == NPCStates.Building)
                    {
                        state = NPCStates.Idle;
                    }
                    else if (state == NPCStates.Gathering)
                    {
                        target = null;
                        state = NPCStates.Idle;
                    }
                    yield break;
                }
            }

            yield return null;
        }
        TargetReached();
    }

    private void TargetReached()
    {
        switch(state)
        {
            case NPCStates.Gathering:
                state = NPCStates.Building;
                cargo = target;
                target = FindObjectOfType<Building>().transform;
                break;
            case NPCStates.Building:
                state = NPCStates.Returning;
                Destroy(cargo.gameObject);
                StopCoroutine(UpdatePath());
                target.GetComponent<Building>().IncreaseCompletion(0.1f);
                state = NPCStates.Idle;
                break;
        }
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
    }

    private static Vector2 V3ToV2(Vector3 V3)
    {
        return new Vector2(V3.x, V3.z);
    }
}
