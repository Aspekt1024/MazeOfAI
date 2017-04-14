using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGrid : MonoBehaviour {

    public bool DisplayGrid;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public int obstacleProximityPenalty = 10;

    [HideInInspector]
    public bool GridGenerated;

    private LayerMask unwalkableMask;
    private LayerMask walkableMask;
    private PathNode[,] grid;
    private float nodeDiameter;
    private int gridSizeX;
    private int gridSizeY;

    private int penaltyMin = int.MaxValue;
    private int penaltyMax = int.MinValue;
    
    private Dictionary<int, int> TerrainPenaltyDict = new Dictionary<int, int>();

    private static List<string> UnwalkableLayers = new List<string>()
    {
        {"MazeWall"},
        {"BuildingBounds"}
    };
    
    public PathNode GetNodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = Mathf.Clamp01((worldPos.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPos.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public PathNode GetNearestWalkableNode(Vector3 worldPos)
    {
        float percentX = Mathf.Clamp01((worldPos.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPos.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        if (grid[x, y].walkable)
            return grid[x, y];
        else if (x > 0 && grid[x - 1, y].walkable)
            return grid[x - 1, y];
        else if (x < gridSizeX - 1 && grid[x + 1, y].walkable)
            return grid[x + 1, y];
        else if (y > 0 && grid[x, y - 1].walkable)
            return grid[x, y - 1];
        else if (y < gridSizeY - 1 && grid[x, y + 1].walkable)
            return grid[x, y + 1];

        for (int dX = -1; dX <= 1; dX++)
        {
            for (int dY = -1; dY <= 1; dY++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = x + dX;
                int checkY = y + dY;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    if (grid[checkX, checkY].walkable)
                        return grid[checkX, checkY];
                }
            }
        }
        return grid[x, y];
    }

    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
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

    private void Awake()
    {
        gameObject.AddComponent<PathRequestManager>();
        SetupUnwalkableMask();
        SetupTerrainPenalties();
    }

    private void SetupUnwalkableMask()
    {
        foreach(string layer in UnwalkableLayers)
        {
            unwalkableMask |= 1 << LayerMask.NameToLayer(layer);
        }
    }

    private void SetupTerrainPenalties()
    {
        TerrainPenaltyDict.Add(LayerMask.NameToLayer("TemporalField"), 20);
        
        foreach (var terrainPenaltyPair in TerrainPenaltyDict)
        {
            walkableMask |= 1 << terrainPenaltyPair.Key;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null && DisplayGrid)
        {
            foreach(PathNode node in grid)
            {
                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, node.movementPenalty));

                Gizmos.color = (node.walkable ? Gizmos.color : Color.red);
                Gizmos.DrawCube(node.worldPosition, new Vector3(1, 0.1f, 1) * (nodeDiameter - 0.05f));
            }
        }
    }

    public void SetupGrid(int rows, int cols)
    {
        float length = MazeWallPlacement.wallLength;
        nodeRadius = length / 12f;
        nodeDiameter = nodeRadius * 2;

        const int numExtraNodes = 30;
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

                int movementPenalty = 0;
                
                Ray ray = new Ray(worldPoint + Vector3.up * 100f, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 200f, walkableMask))
                {
                    movementPenalty = TerrainPenaltyDict[hit.collider.gameObject.layer];
                }

                if (!walkable)
                {
                    movementPenalty += obstacleProximityPenalty;
                }

                grid[x, y] = new PathNode(walkable, worldPoint, x, y, movementPenalty);
            }
        }
        BlurPenalties(1);
        GridGenerated = true;
    }

    private void BlurPenalties(int blurSize)
    {
        int kernelSize = 2 * blurSize + 1;
        int kernelExtents = blurSize;

        int[,] hPassPenalties = new int[gridSizeX, gridSizeY];
        int[,] vPassPenalties = new int[gridSizeX, gridSizeY];

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, gridSizeX - 1);
                hPassPenalties[0, y] += grid[sampleX, y].movementPenalty;
            }

            for (int x = 1; x < gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX - 1);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);
                hPassPenalties[x, y] = hPassPenalties[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, gridSizeY - 1);
                vPassPenalties[x, 0] += hPassPenalties[x, sampleY];
            }

            int blurredPenalty = Mathf.RoundToInt((float)vPassPenalties[x, 0] / (kernelSize * kernelSize));
            grid[x, 0].movementPenalty = blurredPenalty;

            for (int y = 1; y < gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY - 1);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);
                vPassPenalties[x, y] = vPassPenalties[x, y - 1] - hPassPenalties[x, removeIndex] + hPassPenalties[x, addIndex];

                blurredPenalty = Mathf.RoundToInt((float)vPassPenalties[x, y] / (kernelSize * kernelSize));
                grid[x, y].movementPenalty = blurredPenalty;

                if (blurredPenalty > penaltyMax)
                    penaltyMax = blurredPenalty;
                if (blurredPenalty < penaltyMin)
                    penaltyMin = blurredPenalty;
            }
        }
    }
}
