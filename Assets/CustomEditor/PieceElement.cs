using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PieceElement : GridElement
{
    public override void AddDot(Vector2Int coordinats, DotElement dot)
    {      
        Vector2Int calculation = new Vector2Int(
            Mathf.Abs(coordinats.x), 
            Mathf.Abs(coordinats.y));

        //Grid size
        if(calculation.x + 1 > gridSize.x)
            gridSize.x = calculation.x + 1;
        if(calculation.y + 1 > gridSize.y)
            gridSize.y = calculation.y + 1;

        base.AddDot(coordinats, dot);
    }
}
