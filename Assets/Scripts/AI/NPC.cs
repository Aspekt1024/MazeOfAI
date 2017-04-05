using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

    public float speed = 5f;

    private AIMovement movement;
    private PathGrid grid;
    private Level level;
    private Transform player;

    private Vector3[] path;
    private int targetIndex;
    private Transform target;

    private void Awake()
    {
        GameObject scriptsObj = GameObject.FindGameObjectWithTag("Scripts");
        level = scriptsObj.GetComponent<Level>();
        grid = scriptsObj.GetComponent<PathGrid>();

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
        CalculatePath();
    }

    private void CalculatePath()
    {
        grid.CreateGrid(level);
        target = player;
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    private IEnumerator FollowPath()
    {
        Vector3 currentWayPoint = path[0];

        while (true)
        {
            if (transform.position.x == currentWayPoint.x && transform.position.z == currentWayPoint.z)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWayPoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentWayPoint.x, transform.position.y, currentWayPoint.z), speed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(path[i], new Vector3(0.3f, 0.01f, 0.3f));

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
