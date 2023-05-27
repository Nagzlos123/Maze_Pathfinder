using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Procedural;

public class MazeGenerator : MonoBehaviour
{
    public int rows = 12;
    public int cols = 12;
    public GameObject CellPrefab;
    public MazeCell[,] myMazeCells;

    public Maze maze
    {
        get;
        private set;
    }
    Stack<Cell> stack = new Stack<Cell>();

    public bool MazeGenerationCompleted
    {
        get;
        private set;
    } = false;

    void Start()
    {
        int START_X = -cols / 2;
        int START_Y = -rows / 2;
        maze = new Maze(rows, cols);
        myMazeCells = new MazeCell[cols, rows];
        for (int i = 0; i < cols; ++i)
        {
            for (int j = 0; j < rows; ++j)
            {
                GameObject obj = Instantiate(CellPrefab);
                obj.transform.parent = transform;
                Cell cell = maze.GetCell(i, j);
                cell.onSetDirFlag = OnCellSetDirFlag;
                obj.transform.position = new Vector3(START_X + cell.x, START_Y + cell.y, 1.0f);
                myMazeCells[i, j] = obj.GetComponent<MazeCell>();
            }
        }
        CreateNewMaze();
    }

    public void CreateNewMaze()
    {
        // Remove the left wall from  the bottom left cell.
        maze.RemoveCellWall(0, 0, Directions.LEFT);
        // Remove the right wall from  the top right cell.
        maze.RemoveCellWall(cols - 1, rows - 1, Directions.RIGHT);
        // Push the first cell into the stack.
        stack.Push(maze.GetCell(0, 0));
        // Generate the maze in a coroutine  so that we can see the progress of the maze generation in progress.
        StartCoroutine(Coroutine_Generate());
    }

    public void HighlightCell(int i, int j, bool flag)
    {
        myMazeCells[i, j].SetHighlight(flag);
    }
    public void RemoveAllHightlights()
    {
        for (int i = 0; i < cols; ++i)
        {
            for (int j = 0; j < rows; ++j)
            {
                myMazeCells[i, j].SetHighlight(false);
            }
        }
    }
    public void OnCellSetDirFlag(int x, int y, Directions direrction, bool flagB)
    {
        myMazeCells[x, y].SetActive(direrction, flagB);
    }

    bool GenerateStep()
    {
        if (stack.Count == 0) return true;
        Cell cell = stack.Peek();
        var neighbours = maze.GetNeighboursNotVisited(cell.x, cell.y);
        if (neighbours.Count != 0)
        {
            var index = 0;
            if (neighbours.Count > 1)
            {
                index = Random.Range(0, neighbours.Count);
            }
            var item = neighbours[index];
            Cell neighbour = item.Item2;
            neighbour.visited = true;
            maze.RemoveCellWall(cell.x, cell.y, item.Item1);
            myMazeCells[cell.x, cell.y].SetHighlight(true);
            stack.Push(neighbour);
        }
        else
        {
            stack.Pop();
            myMazeCells[cell.x, cell.y].SetHighlight(false);
        }
        return false;
    }

    IEnumerator Coroutine_Generate()
    {
        bool flag = false;
        while (!flag)
        {
            flag = GenerateStep();
            //yield return null;
            yield return new WaitForSeconds(0.05f);
        }
        MazeGenerationCompleted = true;
    } 
}
