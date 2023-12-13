using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class LevelBoard
{
    public DotType[] dots;
    public bool[] activeCells;

    public LevelBoard(List<CellElement> cells, Vector2 boardSize)
    {
        dots = new DotType[cells.Count];
        activeCells = new bool[cells.Count];

        for (int i = 0; i < cells.Count; i++)
        {
            if (!cells[i].turnedOff)
            {
                activeCells[i] = true;

                DotElement occupying = cells[i].holding;
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
