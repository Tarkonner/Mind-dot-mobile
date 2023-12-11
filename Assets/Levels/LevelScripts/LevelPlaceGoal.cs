using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]

public class LevelPlaceGoal
{
    public DotType type;
    public Vector2 goalPosition;

    public LevelPlaceGoal(Vector2 pos, DotType type)
    {
        this.type = type;
        goalPosition = pos;
    }
}
