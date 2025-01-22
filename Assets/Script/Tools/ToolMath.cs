using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ToolMath 
{
    public static float Difference(float a, float b)
    {
        return MathF.Max(a, b) - MathF.Min(a, b);
    }

    public static Vector2Int[] PerpendicularVectors(Vector2Int currentVector)
    {
        Vector2Int[] calculation = new Vector2Int[2];
        calculation[0] = new Vector2Int(-currentVector.y, currentVector.x);
        calculation[1] = new Vector2Int(currentVector.y, -currentVector.x);
        return calculation;
    }
}
