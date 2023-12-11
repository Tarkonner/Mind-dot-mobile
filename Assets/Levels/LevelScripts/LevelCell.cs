using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]

public class LevelCell
{
    public DotType spawnDot;

    public LevelCell(Cell cell)
    {
        if (cell.occupying is Dot dot)
        {
            spawnDot = dot.dotType;
            Debug.Log("Dot not null!");
        }
        else
        {
            spawnDot = DotType.Null;
        }
    }
    public LevelCell(CellElement cell)
    {
        if (cell.holding != null)
        {
            spawnDot = cell.holding.dotType;
            Debug.Log("Dot not null!");
        }
        else
        {
            spawnDot = DotType.Null;
        }
    }
}
