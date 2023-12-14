using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class LevelBoard
{
    public DotType[] dots;
    public bool[] activeCells;
    public Vector2 boardSize;

    public LevelBoard(List<CellElement> cells, Vector2 boardSize)
    {
        this.boardSize = boardSize;

        int boardTotalLengh = (int)(boardSize.x * boardSize.y);

        dots = new DotType[boardTotalLengh];
        activeCells = new bool[boardTotalLengh];

        List<CellElement> cellsToCheck = new List<CellElement>();
        for (int y = 0; y < boardSize.y; y++)
        {
            for (int x = 0; x < boardSize.x; x++)
            {
                cellsToCheck.Add(cells[y * 7 + x]);
            }
        }

        for (int i = 0; i < boardTotalLengh; i++)
        {
            if (!cellsToCheck[i].turnedOff)
            {
                activeCells[i] = true;

                DotElement occupying = cellsToCheck[i].holding;
                if (occupying == null)
                    dots[i] = DotType.Null;
                else
                    dots[i] = occupying.dotType;
            }
            else
            {
                activeCells[i] = false;
            }
        }
    }
}
