using SharedData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PieceElement : GridElement
{
    

    public PieceElement(LevelEditor editor) : base(editor)
    {
    }

    public void ChangeRotationStatus()
    {        
        if (gridData is not PieceData)
            Debug.LogError("Not a piece to rotate");
        

        PieceData pd = (PieceData)gridData;

        pd.canRotate = !pd.canRotate;

        if(pd.canRotate)
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
        gridData = new PieceData();

        bool legalPiece = true;

        foreach (Vector2Int dotPos in gridData.dotDictionary.Keys)
        {
            if (!gridData.dotDictionary[dotPos].isConnected)
            {
                List<Vector2Int> diagonalList = new List<Vector2Int>();
                bool foundAdjacent = false;

                foreach (Vector2Int otherDot in gridData.dotDictionary.Keys)
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
                        gridData.dotDictionary[dotPos].isConnected = true;
                        gridData.dotDictionary[otherDot].isConnected = true;
                    }
                }
                if (!foundAdjacent)
                {
                    foreach (var diagonal in diagonalList)
                    {
                        gridData.dotDictionary[dotPos].isConnected = true;
                        gridData.dotDictionary[diagonal].isConnected = true;
                    }
                }
            }
        }
        foreach (DotData dot in gridData.dotDictionary.Values)
        {
            if (!dot.isConnected)
                legalPiece = false;
        }

        if (legalPiece)
            base.Construct();
        else
            Debug.Log("Error in making piece. Illegal dot placement.");
    }
}
