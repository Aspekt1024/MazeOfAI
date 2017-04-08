using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {
    
    public float speed = 15f;
    public float turnDist = 0.5f;
    public float turnSpeed = 3f;
    public float stoppingDist = 1.5f;

    private AIMovement movement;
    private PathGrid grid;
    private Level level;
    private Transform player;

    private Transform target;
    private Path path;

    private const float pathUpdateMoveThreshold = 0.5f;
    private const float minPathUpdateTime = 0.2f;

    private void Awake()
    {
        GameObject scriptsObj = GameObject.FindGameObjectWithTag("Scripts");
        level = scriptsObj.GetComponent<Level>();
        grid = scriptsObj.GetComponent<PathGrid>(); // TODO move out of here and into pathfinder manageR?

        player = GameObject.Find("Player").transform;

        movement = gameObject.AddComponent<AIMovement>();
    }


    private void Start()
    {
        StartCoroutine(SetupNPC());
        transform.position = new Vector3(MazeWallPlacement.xStart + 10f, 0f, MazeWallPlacement.zStart + level.numRows * MazeWallPlacement.wallLength + 3f);
    }

    private IEnumerator SetupNPC()
    {
        float waitTime = 0f;
        const float timeBeforeStart = 1f;
        while (waitTime < timeBeforeStart)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }
        target = player;
        grid.CreateGrid(level); // TODO move out of this and into pathfinder
        StartCoroutine(UpdatePath());
    }

    private IEnumerator UpdatePath()
    {
        float squareMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetOldPos = target.position;
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);

        while (true)
        {
            if (Time.timeSinceLevelLoad < 0.3f) yield return new WaitForSeconds(0.3f);
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((target.position - targetOldPos).sqrMagnitude > squareMoveThreshold)
            {
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
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

            if (path.lookPoints.Length > 0)
                StartCoroutine("FollowPath");
        }
    }

    private IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        float speedPercent = 1f;
        transform.LookAt(path.lookPoints[0]);

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
                    if (speedPercent < 0.01f)
                        followingPath = false;
                }


                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - new Vector3(pos2D.x, 0, pos2D.y));
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
            }
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }
}
