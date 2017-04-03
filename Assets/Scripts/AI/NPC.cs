using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

    public float speed = 5f;

    private AIMovement movement;
    private Pathfinder pathFinder;
    private PathGrid grid;
    private Level level;
    private Transform player;

    private List<PathNode> path;

    private void Awake()
    {
        GameObject scriptsObj = GameObject.FindGameObjectWithTag("Scripts");
        level = scriptsObj.GetComponent<Level>();
        grid = scriptsObj.GetComponent<PathGrid>();

        player = GameObject.Find("Player").transform;

        movement = gameObject.AddComponent<AIMovement>();
        pathFinder = gameObject.AddComponent<Pathfinder>();
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

        waitTime = 0f;
        while (waitTime < timeBeforeStart)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }
        StartMovement();
    }

    private void CalculatePath()
    {
        // TODO this should only be called once, but we will want to call it multiple times throughout the game
        grid.CreateGrid(level);
        path = pathFinder.GetPath(transform.position, player.position);
    }

    private void StartMovement()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }
}
