using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeWallPlacement
{
    private const string MazeWallPath = "MazePieces/MazeWall";
    private GameObject mazeWallRef;
    private Transform mazeParent;

    public const float xStart = 140f;
    public const float zStart = 20f;

    public const float wallLength = 2f;
    public const float wallWidth = 0.5f;
    public const float wallHeight = 2f;
    
    public void Load()
    {
        mazeWallRef = Resources.Load<GameObject>(MazeWallPath);
        mazeParent = GameObject.Find("Maze").transform;
    }

    public void PlaceCell(MazeGenerator.MazeCell cell, int row, int col)
    {
        float xPos = xStart + col * wallLength;
        float zPos = zStart + row * wallLength;
        float yPos = wallHeight / 2f;
        
        if (cell.northWall != null)
        {
            cell.northWall.transform.localScale = new Vector3(wallLength, wallHeight, wallWidth);
            cell.northWall.transform.position = new Vector3(xPos, yPos, zPos + wallLength / 2f);
        }
        
        if (cell.eastWall != null)
        {
            cell.eastWall.transform.localScale = new Vector3(wallWidth, wallHeight, wallLength);
            cell.eastWall.transform.position = new Vector3(xPos + wallLength / 2f, yPos, zPos);
        }

        if (col == 0 && cell.westWall != null)
        {
            cell.westWall.transform.localScale = new Vector3(wallWidth, wallHeight,wallLength);
            cell.westWall.transform.position = new Vector3(xPos - wallLength / 2, yPos, zPos);
        }

        if (row == 0 && cell.southWall != null)
        {
            cell.southWall.transform.localScale = new Vector3(wallLength, wallHeight, wallWidth);
            cell.southWall.transform.position = new Vector3(xPos, yPos, zPos - wallLength / 2f);
        }
    }

    public GameObject GetNewWall()
    {
        return Object.Instantiate(mazeWallRef, mazeParent);
    }
}
