using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
[Serializable]
public class LevelBoard
{
    public LevelCell[,] levelGrid;

    public LevelPlaceGoal[] levelPG;

    public LevelBoard(Board board)
    {
        levelGrid = new LevelCell[board.grid.GetLength(0),board.grid.GetLength(1)];
        int i = 0;
        for (int x = 0; x < board.grid.GetLength(0); x++)
        {
            for (int y = 0; y < board.grid.GetLength(1); y++)
            {
                if (board.grid[x, y] != null)
                {
                    levelGrid[x,y] = new LevelCell(board.grid[x,y]);
                    i++;
                }
            }
        }
    }
    public LevelBoard(List<CellElement> cells, Vector2 boardSize)
    {

        levelGrid=new LevelCell[(int)boardSize.x, (int)boardSize.y];
        Debug.Log(boardSize);
        for (int i = 0; i < cells.Count; i++)
        {
            Debug.Log(cells[i].gridCoordinates);
            levelGrid[cells[i].gridCoordinates.x,cells[i].gridCoordinates.y] = new LevelCell(cells[i].cell);
        }
    }
}
