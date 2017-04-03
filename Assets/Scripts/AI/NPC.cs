using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {

    public float speed = 5f;

    private AIMovement movement;

    private Pathfinder pathFinder;
    private PathGrid grid;
    private Level level;

    private void Awake()
    {
        GameObject scriptsObj = GameObject.FindGameObjectWithTag("Scripts");
        level = scriptsObj.GetComponent<Level>();
        grid = scriptsObj.GetComponent<PathGrid>();

        movement = gameObject.AddComponent<AIMovement>();
    }


    private void Start()
    {
        StartCoroutine(SetupNPC());
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
        PlaceAtStartingPoint();

        waitTime = 0f;
        while (waitTime < timeBeforeStart)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }
        StartMovement();
    }

    private void PlaceAtStartingPoint()
    {
        grid.CreateGrid(level);
        pathFinder = gameObject.AddComponent<Pathfinder>();

    }

    private void StartMovement()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }
}
