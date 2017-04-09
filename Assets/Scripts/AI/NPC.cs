using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {
    
    public float speed = 15f;
    public float turnDist = 0.5f;
    public float turnSpeed = 3f;
    public float stoppingDist = 2.5f;

    private AIMovement movement;
    private PathGrid grid;
    private Level level;
    private Transform player;

    private Transform target;
    private Path path;

    private const float pathUpdateMoveThreshold = 0.7f;
    private const float minPathUpdateTime = 0.25f;

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
        const float timeBeforeStart = 1f;
        yield return new WaitForSeconds(timeBeforeStart);
        target = player;
        grid.CreateGrid(level); // TODO move out of this and into pathfinder
        StartCoroutine(UpdatePath());
    }

    private IEnumerator UpdatePath()
    {
        float squareMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetOldPos = target.position;
        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, gameObject, OnPathFound));

        while (true)
        {
            if (Time.timeSinceLevelLoad < 0.5f) yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(minPathUpdateTime);
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
                    if (speedPercent < 0.01f || Vector3.Distance(player.position, transform.position) < 0.8f)
                        followingPath = false;
                }
                
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(path.lookPoints[pathIndex].x, 0, path.lookPoints[pathIndex].z) - new Vector3(pos2D.x, 0, pos2D.y));
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.rotation = Quaternion.LookRotation(new Vector3(transform.forward.x, -0.4f * speedPercent, transform.forward.z));
                transform.position += new Vector3(transform.forward.x, 0, transform.forward.z) * Time.deltaTime * speed * speedPercent;
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
