using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGridSection : Section {
    
    [HideInInspector]
    public PathGrid grid;

    public void GenerateGrid()
    {
        grid = gameObject.AddComponent<PathGrid>();
        grid.CreateGrid(new Vector2(cols, rows), transform.position);

        Vector2 gridHalfSize = grid.gridWorldSize / 2f;
        lowerBound = new Vector2(transform.position.x - gridHalfSize.x, transform.position.z - gridHalfSize.y);
        upperBound = new Vector2(transform.position.x + gridHalfSize.x, transform.position.z + gridHalfSize.y);
    }
    
    protected virtual void OnDrawGizmos()
    {
        Level level = GameObject.FindGameObjectWithTag("Scripts").GetComponent<Level>();
        AlignSectionToGrid();

        const float cubeHeight = 0.1f;
        Gizmos.DrawWireCube(transform.position + Vector3.up * cubeHeight / 2, new Vector3(cols * level.SegmentLength, cubeHeight, rows * level.SegmentLength));
        
    }
}
