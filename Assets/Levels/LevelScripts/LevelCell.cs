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
        }
        else
        {
            spawnDot = DotType.Null;
        }
    }
}
