using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAI.PathFinding;
using System;
namespace Procedural
{
    // The directions types for maze
    // propagation.
    public enum Directions
    {
        UP,
        RIGHT,
        DOWN,
        LEFT,
        NONE,
    }


    // A cell in the maze.
    public class Cell : Node<Vector2Int>
    {
        public int x { get { return Value.x; } }
        public int y { get { return Value.y; } }

       
        // visited flag. 
        // it is used while creating the maze.
        public bool visited = false;
        public bool[] flag = {true, true, true, true };
        // a delegate that is called
        // when we set a direction flag 
        // to the cell.
        public delegate void DelegateSetDirFlag(int x, int y, Directions directon, bool flagB);
        public DelegateSetDirFlag onSetDirFlag;

        public void SetDirFlag(Directions directon, bool flagB)
        {
            flag[(int)directon] = flagB;
            onSetDirFlag?.Invoke(x, y, directon, flagB);
        }

        private Maze mMaze;


        // constructor
        public Cell(int c, int r, Maze maze) : base(new Vector2Int(c, r))
        {


            mMaze = maze;
        }
        // get the neighbours for this cell.
        // here will will just throw the responsibility
        // to get the neighbours to the grid.
        public override List<Node<Vector2Int>> GetNeighbours()
        {
            List<Node<Vector2Int>> neighbours = new List<Node<Vector2Int>>();
            foreach (Directions dir in Enum.GetValues(typeof(Directions)))
            {
                int x = Value.x;
                int y = Value.y;
                switch (dir)
                {
                    case Directions.UP:
                        if (y < mMaze.NumRows - 1)
                        {
                            ++y;
                            if (!flag[(int)dir])
                            {
                                neighbours.Add(mMaze.GetCell(x, y));
                            }
                        }
                        break;
                    case Directions.RIGHT:
                        if (x < mMaze.NumCols - 1)
                        {
                            ++x;
                            if (!flag[(int)dir])
                            {
                                neighbours.Add(mMaze.GetCell(x, y));
                            }
                        }
                        break;
                    case Directions.DOWN:
                        if (y > 0)
                        {
                            --y;
                            if (!flag[(int)dir])
                            {
                                neighbours.Add(mMaze.GetCell(x, y));
                            }
                        }
                        break;
                    case Directions.LEFT:
                        if (x > 0)
                        {
                            --x;
                            if (!flag[(int)dir])
                            {
                                neighbours.Add(mMaze.GetCell(x, y));
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return neighbours;
        }
    }
}
