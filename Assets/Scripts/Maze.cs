using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Procedural
{
    public class Maze
    {

        private int numberOfRows;
        private int numberOfCols;

        private Cell[,] numberOfCells;


        public int NumRows { get { return numberOfRows; } }
        public int NumCols { get { return numberOfCols; } }

        // constructor
        public Maze(int rows, int cols)
        {
            numberOfRows = rows;
            numberOfCols = cols;
            // initiaze all the cells.
            numberOfCells = new Cell[numberOfCols, numberOfRows];
            for (var i = 0; i < numberOfCols; i++)
            {
                for (var j = 0; j < numberOfRows; j++)
                {
                    numberOfCells[i, j] = new Cell(i, j, this);
                }
            }

        }
        public Cell GetCell(int i, int j)
        {
            return numberOfCells[i, j];
        }

        public int GetCellCount()
        {
            return numberOfRows * numberOfCols;
        }

        public List<Tuple<Directions, Cell>>
            GetNeighboursNotVisited(int cellX, int cellY)
        {
            List<Tuple<Directions, Cell>> neighbours = new List<Tuple<Directions, Cell>>();
            foreach (Directions direction in Enum.GetValues(typeof(Directions)))
            {
                int x = cellX;
                int y = cellY;
                switch (direction)
                {
                    case Directions.UP:
                        if (y < numberOfRows - 1)
                        {
                            ++y;
                            if (!numberOfCells[x, y].visited)
                            {
                                neighbours.Add(new Tuple<Directions, Cell>(
                                  Directions.UP,
                                  numberOfCells[x, y])
                                );
                            }
                        }
                        break;

                    case Directions.RIGHT:
                        if (x < numberOfCols - 1)
                        {
                            ++x;
                            if (!numberOfCells[x, y].visited)
                            {
                                neighbours.Add(new Tuple<Directions, Cell>(
                                  Directions.RIGHT,
                                  numberOfCells[x, y])
                                );
                            }
                        }
                        break;

                    case Directions.DOWN:
                        if (y > 0)
                        {
                            --y;
                            if (!numberOfCells[x, y].visited)
                            {
                                neighbours.Add(new Tuple<Directions, Cell>(
                                  Directions.DOWN,
                                  numberOfCells[x, y])
                                );
                            }
                        }
                        break;
                    case Directions.LEFT:
                        if (x > 0)
                        {
                            --x;
                            if (!numberOfCells[x, y].visited)
                            {
                                neighbours.Add(new Tuple<Directions, Cell>(
                                  Directions.LEFT,
                                  numberOfCells[x, y])
                                );
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return neighbours;
        }

        public void RemoveCellWall(int x, int y, Directions direction)
        {
            if (direction != Directions.NONE)
            {
                Cell cell = GetCell(x, y);
                cell.SetDirFlag(direction, false);

                Directions opp = Directions.NONE;
                switch (direction)
                {
                    case Directions.UP:
                        if (y < numberOfRows - 1)
                        {
                            opp = Directions.DOWN;
                            ++y;
                        }
                        break;

                    case Directions.RIGHT:
                        if (x < numberOfCols - 1)
                        {
                            opp = Directions.LEFT;
                            ++x;
                        }
                        break;

                    case Directions.DOWN:
                        if (y > 0)
                        {
                            opp = Directions.UP;
                            --y;
                        }
                        break;

                    case Directions.LEFT:
                        if (x > 0)
                        {
                            opp = Directions.RIGHT;
                            --x;
                        }
                        break;
                }

                if (opp != Directions.NONE)
                {
                    Cell cell1 = GetCell(x, y);
                    cell1.SetDirFlag(opp, false);
                }
            }
        }
    }
}