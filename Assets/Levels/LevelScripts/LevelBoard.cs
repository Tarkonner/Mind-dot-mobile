using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBoard
{
    public LevelCell[,] levelGrid;

    public LevelBoard(Board board)
    {
        levelGrid = new LevelCell[board.grid.GetLength(0), board.grid.GetLength(1)];
        for (int x = 0; x < board.grid.GetLength(0); x++)
        {
            for (int y = 0; y < board.grid.GetLength(1); y++)
            {
                if (board.grid[x, y] != null)
                {
                    levelGrid[x, y] = new LevelCell(board.grid[x,y]);
                }
            }
        }
    }
}
