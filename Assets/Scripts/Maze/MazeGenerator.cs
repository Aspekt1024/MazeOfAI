using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator {
    
    public class MazeCell
    {
        public bool visited;
        public int row;
        public int col;
        public bool northWall;
        public bool southWall;
        public bool eastWall;
        public bool westWall;
    }

    private Maze mazeScript;
    private MazeWallPlacement wallHandler = new MazeWallPlacement();
    private MazeCell[,] maze;
    private int rows;
    private int cols;

    private class CellPos
    {
        public int r;
        public int c;

        public CellPos(int row, int col)
        {
            r = row;
            c = col;
        }
    }
    private CellPos currentCell;

    public MazeGenerator()
    {
        mazeScript = GameObject.FindGameObjectWithTag("Maze").GetComponent<Maze>();
    }

    public void Generate(int numRows, int numCols, int startCol = -1, int endCol = -1)
    {
        wallHandler.LoadMaze(mazeScript);

        if (numRows < 1 || numCols < 1)
        {
            Debug.Log("Maze must have at least 3 rows and 3 columns.");
            return;
        }

        rows = numRows;
        cols = numCols;
        SetupNewMaze();

        if (startCol == -1) startCol = Random.Range(0, cols);
        if (endCol == -1) endCol = Random.Range(0, cols);
        maze[0, startCol].southWall = false;
        maze[rows - 1, endCol].northWall = false;

        currentCell = new CellPos(0, startCol);

        CreateMaze();
        DeployMaze();
    }

    private void CreateMaze()
    {
        maze[currentCell.r, currentCell.c].visited = true;
        List<CellPos> cellStack = new List<CellPos>();
        cellStack.Add(currentCell);
        
        while (cellStack.Count > 0)
        {
            List<CellPos> neighbours = GetUnvisitedNeighbours();

            if (neighbours.Count > 0)
            {
                int neighbourIndex = Random.Range(0, neighbours.Count);
                CellPos tempCell = neighbours[neighbourIndex];
                RemoveIntersectingWall(tempCell, currentCell);
                maze[tempCell.r, tempCell.c].visited = true;
                cellStack.Add(currentCell);
                currentCell = tempCell;
            }
            else
            {
                int cellIndex = Random.Range(0, cellStack.Count);
                currentCell = cellStack[cellIndex];
                cellStack.Remove(cellStack[cellIndex]);
            }
        }
    }

    private void RemoveIntersectingWall(CellPos temp, CellPos current)
    {
        if (temp.c == current.c - 1)
        {
            maze[current.r, current.c].westWall = false;
            maze[temp.r, temp.c].eastWall = false;
        }
        else if (temp.c == current.c + 1)
        {
            maze[current.r, current.c].eastWall = false;
            maze[temp.r, temp.c].westWall = false;
        }
        else if (temp.r == current.r - 1)
        {
            maze[current.r, current.c].southWall = false;
            maze[temp.r, temp.c].northWall = false;
        }
        else if (temp.r == current.r + 1)
        {
            maze[current.r, current.c].northWall = false;
            maze[temp.r, temp.c].southWall = false;
        }
    }

    private List<CellPos> GetUnvisitedNeighbours()
    {
        List<CellPos> neighbours = new List<CellPos>();

        if (currentCell.r > 0)
        {
            if (!maze[currentCell.r - 1, currentCell.c].visited)
                neighbours.Add(new CellPos(currentCell.r - 1, currentCell.c));
        }
        if (currentCell.r < rows - 1)
        {
            if (!maze[currentCell.r + 1, currentCell.c].visited)
                neighbours.Add(new CellPos(currentCell.r + 1, currentCell.c));
        }
        if (currentCell.c > 0)
        {
            if (!maze[currentCell.r, currentCell.c - 1].visited)
                neighbours.Add(new CellPos(currentCell.r, currentCell.c - 1));
        }
        if (currentCell.c < cols - 1)
        {
            if (!maze[currentCell.r, currentCell.c + 1].visited)
                neighbours.Add(new CellPos(currentCell.r, currentCell.c + 1));
        }
        
        return neighbours;
    }

    private void DeployMaze()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                wallHandler.PlaceCell(maze[row, col], row, col);
            }
        }
    }

    private void SetupNewMaze()
    {
        maze = new MazeCell[rows, cols];
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                maze[row, col] = new MazeCell();

                maze[row, col].southWall = row > 0 ? maze[row - 1, col].northWall : true;
                maze[row, col].westWall = col > 0 ? maze[row, col - 1].eastWall : true;
                maze[row, col].eastWall = true;
                maze[row, col].northWall = true;
            }
        }
    }
}
