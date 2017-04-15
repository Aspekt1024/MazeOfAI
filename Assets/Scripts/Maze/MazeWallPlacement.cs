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

    private Maze mazeScript;
    private MazeGenerator mazeGen;
    
    public void LoadMaze(Maze mazeRef)
    {
        mazeScript = mazeRef;
        mazeWallRef = Resources.Load<GameObject>(MazeWallPath);
        mazeParent = mazeScript.transform;

        wallLength = mazeScript.wallLength;
        wallHeight = mazeScript.wallHeight;
        wallWidth = mazeScript.wallWidth;

        xStart = mazeScript.transform.position.x - (mazeScript.cols - 1) * wallLength / 2;
        zStart = mazeScript.transform.position.z - (mazeScript.rows - 1) * wallLength / 2;
    }
    
    public void DeployMaze(MazeGenerator mazeGenRef)
    {
        mazeGen = mazeGenRef;
        for (int row = 0; row < mazeScript.rows; row++)
        {
            for (int col = 0; col < mazeScript.cols; col++)
            {
                PlaceCell(row, col);
            }
        }
    }

    private void PlaceCell(int row, int col)
    {
        float xPos = xStart + col * wallLength;
        float zPos = zStart + row * wallLength;
        float yPos = wallHeight / 2f;

        if (mazeGen.Maze[row, col].northWall)
        {
            PosScalePair psp = GetNorthWallTf(row, col);
            PlaceNewWall(psp);
        }
        
        if (mazeGen.Maze[row, col].eastWall)
        {
            PosScalePair psp = GetEastWallTf(row, col);
            PlaceNewWall(psp);
        }

        if (col == 0 && mazeGen.Maze[row, col].westWall)
        {
            PosScalePair psp = GetWestWallTf(row, col);
            PlaceNewWall(psp);
        }

        if (row == 0 && mazeGen.Maze[row, col].southWall)
        {
            PosScalePair psp = GetSouthWallTf(row, col);
            PlaceNewWall(psp);
        }
    }

    private void PlaceNewWall(PosScalePair psp)
    {
        GameObject newWall = Object.Instantiate(mazeWallRef, mazeParent);
        newWall.transform.position = psp.pos;
        newWall.transform.localScale = psp.scale;
    }

    private PosScalePair GetWestWallTf(int row, int col)
    {
        float xPos = xStart + col * wallLength;
        float zPos = zStart + row * wallLength;
        float yPos = wallHeight / 2f;

        PosScalePair psp = new PosScalePair()
        {
            pos = new Vector3(xPos - wallLength / 2, yPos, zPos),
            scale = new Vector3(wallWidth, wallHeight, wallLength)
        };

        // West wall is only placed when col == 0
        if (mazeGen.Maze[row, col].northWall)
        {
            psp.scale += Vector3.back * wallWidth / 2;
            psp.pos += Vector3.back * wallWidth / 4;
        }
        if (mazeGen.Maze[row, col].southWall)
        {
            psp.scale += Vector3.back * wallWidth / 2;
            psp.pos += Vector3.forward * wallWidth / 4;
        }

        return psp;
    }

    private PosScalePair GetEastWallTf(int row, int col)
    {
        float xPos = xStart + col * wallLength;
        float zPos = zStart + row * wallLength;
        float yPos = wallHeight / 2f;

        PosScalePair psp = new PosScalePair()
        {
            pos = new Vector3(xPos + wallLength / 2f, yPos, zPos),
            scale = new Vector3(wallWidth, wallHeight, wallLength)
        };

        if (mazeGen.Maze[row, col].northWall || (col < mazeScript.cols - 1 && mazeGen.Maze[row, col + 1].northWall))
        {
            psp.scale += Vector3.back * wallWidth / 2;
            psp.pos += Vector3.back * wallWidth / 4;
        }
        if (mazeGen.Maze[row, col].southWall || (col < mazeScript.cols - 1 && mazeGen.Maze[row, col + 1].southWall))
        {
            psp.scale += Vector3.back * wallWidth / 2;
            psp.pos += Vector3.forward * wallWidth / 4;
        }

        return psp;
    }

    private PosScalePair GetNorthWallTf(int row, int col)
    {
        float xPos = xStart + col * wallLength;
        float zPos = zStart + row * wallLength;
        float yPos = wallHeight / 2f;

        PosScalePair psp = new PosScalePair()
        {
            pos = new Vector3(xPos, yPos, zPos + wallLength / 2f),
            scale = new Vector3(wallLength, wallHeight, wallWidth)
        };

        if (col == 0 && row == mazeScript.rows - 1)
        {
            psp.scale += Vector3.right * wallWidth / 2;
            psp.pos += Vector3.left * wallWidth / 4;
        }
        else if (col == mazeScript.cols - 1 && row == mazeScript.rows - 1)
        {
            psp.scale += Vector3.right * wallWidth / 2;
            psp.pos += Vector3.right * wallWidth / 4;
        }
        else
        {
            if ((mazeGen.Maze[row, col].eastWall && !(col < mazeScript.cols - 1 && mazeGen.Maze[row, col + 1].northWall))
                || (row < mazeScript.rows - 1 && (mazeGen.Maze[row + 1, col].eastWall && !(col < mazeScript.cols - 1 && mazeGen.Maze[row + 1, col + 1].southWall))))
            {
                psp.scale += Vector3.right * wallWidth / 2;
                psp.pos += Vector3.right * wallWidth / 4;
            }
            if ((mazeGen.Maze[row, col].westWall && !(col > 0 && mazeGen.Maze[row, col - 1].northWall))
                || (row < mazeScript.rows - 1 && (mazeGen.Maze[row + 1, col].westWall && !(col > 0 && mazeGen.Maze[row + 1, col - 1].southWall))))
            {
                psp.scale += Vector3.right * wallWidth / 2;
                psp.pos += Vector3.left * wallWidth / 4;
            }
        }
        return psp;
    }

    private PosScalePair GetSouthWallTf(int row, int col)
    {
        float xPos = xStart + col * wallLength;
        float zPos = zStart + row * wallLength;
        float yPos = wallHeight / 2f;

        PosScalePair psp = new PosScalePair()
        {
            pos = new Vector3(xPos, yPos, zPos - wallLength / 2f),
            scale = new Vector3(wallLength, wallHeight, wallWidth)
        };

        // South walls are only placed when row == 0
        if (col == 0)
        {
            psp.scale += Vector3.right * wallWidth / 2;
            psp.pos += Vector3.left * wallWidth / 4;
        }
        else if (col == mazeScript.cols - 1)
        {
            psp.scale += Vector3.right * wallWidth / 2;
            psp.pos += Vector3.right * wallWidth / 4;
        }
        else
        {
            if (mazeGen.Maze[row, col].westWall && !(col > 0 && mazeGen.Maze[row, col - 1].southWall))
            {
                psp.scale += Vector3.right * wallWidth / 2;
                psp.pos += Vector3.left * wallWidth / 4;
            }
            if (mazeGen.Maze[row, col].eastWall && !(col < mazeScript.cols - 1 && mazeGen.Maze[row, col + 1].southWall))
            {
                psp.scale += Vector3.right * wallWidth / 2;
                psp.pos += Vector3.right * wallWidth / 4;
            }
        }

        return psp;
    }

    private struct PosScalePair
    {
        public Vector3 pos;
        public Vector3 scale;
    }
}
