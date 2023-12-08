using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PieceElement : GridElement
{
    private bool canRotate = true;

    public PieceElement(LevelEditor editor) : base(editor)
    {
    }

    public void ChangeRotationStatus()
    {
        canRotate = !canRotate;

        if(canRotate)
        {
            foreach (Image i in images)
                i.tintColor = Color.white;
        }
        else
        {
            foreach(Image i in images)
                i.tintColor = Color.cyan;
        }    
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
