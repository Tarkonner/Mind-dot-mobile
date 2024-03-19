using SharedData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PieceElement : GridElement
{
    public override void Construct(Vector2Int targetSize)
    {
        PieceData temp = new PieceData();
        temp.dotDictionary = gridData.dotDictionary;
        temp.gridSize = gridData.gridSize;
        temp.gridPosRef = gridData.gridPosRef;
        gridData = temp;

        //Piece legality test
        bool legalPiece = true;

        //Setup data
        List<Vector2Int> dotPos = new List<Vector2Int>();
        foreach (Vector2Int key in gridData.dotDictionary.Keys)
            dotPos.Add(key);

        //Gate
        if (dotPos.Count == 0)
            legalPiece = false;

        int connectionMade = 0;
        Stack<Vector2Int> neighbors = new Stack<Vector2Int>();
        Vector2Int calculationPoint = dotPos[0];
        dotPos.RemoveAt(0);
        while (legalPiece)
        {
            //Look for nieghbors
            for (int i = dotPos.Count - 1; i >= 0; i--)
            {
                //Do we have the point
                if (neighbors.Contains(dotPos[i]))
                    continue;

                if(Vector2.Distance(calculationPoint, dotPos[i]) < 1.5f)
                {
                    neighbors.Push(dotPos[i]);
                    dotPos.RemoveAt(i);
                    connectionMade++;
                }
            }

            if(neighbors.Count == 0)
            {
                if(connectionMade < gridData.dotDictionary.Keys.Count - 1)
                {
                    legalPiece = false;
                    Debug.Log("Error in making piece. Illegal dot placement.");
                }

                break;
            }
            else
            {
                calculationPoint = neighbors.Pop();
            }
        }

        if (legalPiece)
            base.Construct(targetSize);
    }
}
