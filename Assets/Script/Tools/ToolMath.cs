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
}
