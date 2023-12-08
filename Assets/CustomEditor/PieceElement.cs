using System;
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
    public override void Construct()
    {
        bool legalPiece = true;

        foreach (Vector2Int dotPos in dotDictionary.Keys)
        {
            if (!dotDictionary[dotPos].isConnected)
            {
                List<Vector2Int> diagonalList = new List<Vector2Int>();
                bool foundAdjacent = false;

                foreach (Vector2Int otherDot in dotDictionary.Keys)
                {
                    if(dotPos == otherDot) { continue; }

                    float val = Vector2Int.Distance(dotPos, otherDot);
                    if (val > 1.5f) continue;

                    else if (val > 1.2f && !foundAdjacent)
                    {
                        diagonalList.Add(otherDot);
                    }
                    else if (val == 1)
                    {
                        foundAdjacent = true;
                        //Create Connection
                        dotDictionary[dotPos].isConnected = true;
                        dotDictionary[otherDot].isConnected = true;
                    }
                }
                if (!foundAdjacent)
                {
                    foreach (var diagonal in diagonalList)
                    {
                        dotDictionary[dotPos].isConnected = true;
                        dotDictionary[diagonal].isConnected = true;
                    }
                }
            }
        }
        foreach (DotElement dot in dotDictionary.Values)
        {
            if (!dot.isConnected)
            {
                legalPiece = false;
            }
        }

        if (legalPiece)
        {
            base.Construct();
        }
        else
        {
            Debug.Log("Error in making piece. Illegal dot placement.");
        }

    }
}
