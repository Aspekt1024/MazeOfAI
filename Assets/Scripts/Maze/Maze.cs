using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {

    public int rows = 10;
    public int cols = 10;
    public int startCol = 8;
    public int endCol = 2;

    public float wallLength = 2f;
    public float wallWidth = 0.5f;
    public float wallHeight = 1.5f;

    public bool DisplayGrid;
    

    [HideInInspector]
    public PathGrid grid;

    private void Awake ()
    {
        MazeGenerator generator = new MazeGenerator();
        generator.Generate(rows, cols, startCol, endCol);
    }

    private void Start()
    {
        grid = gameObject.AddComponent<PathGrid>();
        grid.CreateGrid(rows, cols);
    }
	
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.up * wallHeight / 2, new Vector3(cols * wallLength, wallHeight, rows * wallLength));

        if (startCol >= cols) startCol = cols - 1;
        if (endCol >= cols) endCol = cols - 1;

        if (startCol >= 0)
            Gizmos.DrawWireCube(transform.position + new Vector3(startCol * wallLength - (cols - 1) * wallLength / 2, wallHeight / 2, -rows * wallLength / 2), new Vector3(wallLength, wallHeight, wallWidth));

        if (endCol >= 0)
            Gizmos.DrawWireCube(transform.position + new Vector3(endCol * wallLength - (cols - 1) * wallLength / 2, wallHeight / 2, rows * wallLength / 2), new Vector3(wallLength, wallHeight, wallWidth));

    }

}
