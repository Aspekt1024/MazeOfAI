using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeWallPlacement
{
    private const string MazeWallPath = "MazePieces/MazeWall";
    private GameObject mazeWallRef;
    private Transform mazeParent;

    public float xStart;
    public float zStart;

    private float wallLength;
    private float wallHeight;
    private float wallWidth;
    
    public void LoadMaze(Maze maze)
    {
        mazeWallRef = Resources.Load<GameObject>(MazeWallPath);
        mazeParent = maze.transform;

        wallLength = maze.wallLength;
        wallHeight = maze.wallHeight;
        wallWidth = maze.wallWidth;

        xStart = maze.transform.position.x - (maze.cols - 1) * wallLength / 2;
        zStart = maze.transform.position.z - (maze.rows - 1) * wallLength / 2;
    }

    public void PlaceCell(MazeGenerator.MazeCell cell, int row, int col)
    {
        float xPos = xStart + col * wallLength;
        float zPos = zStart + row * wallLength;
        float yPos = wallHeight / 2f;

        if (cell.northWall == true)
        {
            Vector3 pos = new Vector3(xPos, yPos, zPos + wallLength / 2f);
            Vector3 scale = new Vector3(wallLength, wallHeight, wallWidth);
            PlaceNewWall(pos, scale);
        }
        
        if (cell.eastWall == true)
        {
            Vector3 pos = new Vector3(xPos + wallLength / 2f, yPos, zPos);
            Vector3 scale = new Vector3(wallWidth, wallHeight, wallLength);
            PlaceNewWall(pos, scale);
        }

        if (col == 0 && cell.westWall == true)
        {
            Vector3 pos = new Vector3(xPos - wallLength / 2, yPos, zPos);
            Vector3 scale = new Vector3(wallWidth, wallHeight, wallLength);
            PlaceNewWall(pos, scale);
        }

        if (row == 0 && cell.southWall == true)
        {
            Vector3 pos = new Vector3(xPos, yPos, zPos - wallLength / 2f);
            Vector3 scale = new Vector3(wallLength, wallHeight, wallWidth);
            PlaceNewWall(pos, scale);
        }
    }

    private void PlaceNewWall(Vector3 pos, Vector3 scale)
    {
        GameObject newWall = Object.Instantiate(mazeWallRef, mazeParent);
        newWall.transform.position = pos;
        newWall.transform.localScale = scale;
    }
}
