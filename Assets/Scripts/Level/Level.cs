using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    public int numRows = 5;
    public int numCols = 5;
    private MazeGenerator mazeGenerator = new MazeGenerator();
    
	private void Start ()
    {
        mazeGenerator.Generate(numRows, numCols);
    }
}
