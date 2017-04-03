using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGrid : MonoBehaviour {
    
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public PathNode[,] grid;
    public List<PathNode> path; // todo delete

    private float nodeDiameter;
    private int gridSizeX;
    private int gridSizeY;

    public PathNode GetNodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = Mathf.Clamp01((worldPos.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPos.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<PathNode> GetNeighbours(PathNode node)
    {
        List<PathNode> neighbours = new List<PathNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    neighbours.Add(grid[checkX, checkY]);
            }
        }
        return neighbours;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            foreach(PathNode node in grid)
            {
                Gizmos.color = (node.walkable ? Color.white : Color.red);
                if (path != null)
                {
                    if (path.Contains(node))
                        Gizmos.color = Color.cyan;
                }
                Gizmos.DrawCube(node.worldPosition, new Vector3(1, 0.1f, 1) * (nodeDiameter - 0.05f));
            }
        }
    }

    public void SetupGrid(int rows, int cols)
    {
        float length = MazeWallPlacement.wallLength;
        nodeRadius = length / 4f;
        nodeDiameter = nodeRadius * 2;

        const int numExtraNodes = 10;
        gridWorldSize = new Vector2(length * (cols + 0.5f), length * (rows + 0.5f) + numExtraNodes * nodeDiameter);
        transform.position = new Vector3(MazeWallPlacement.xStart + length * (cols - 1) / 2f, 0f, MazeWallPlacement.zStart + length * (rows - 1) / 2f);

        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
    }

    public void CreateGrid(Level level)
    {
        SetupGrid(level.numRows, level.numCols);

        grid = new PathNode[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new PathNode(walkable, worldPoint, x, y);
            }
        }
    }
}
