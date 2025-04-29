using SharedData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PieceElement : GridElement
{
    public VisualElement holder;
    public PieceData PieceData => gridData as PieceData;

    public override void Construct(Vector2Int targetSize)
    {
        PieceData temp = new PieceData();
        temp.dotDictionary = gridData.dotDictionary;
        temp.gridSize = gridData.gridSize;
        temp.gridPosRef = gridData.gridPosRef;
        gridData = temp;


        //Setup data
        List<Vector2Int> dotPos = new List<Vector2Int>();
        foreach (Vector2Int key in gridData.dotDictionary.Keys)
            dotPos.Add(key);

        //Gate
        if (dotPos.Count == 0)
        {
            Debug.LogError("No Dots in Piece");
            return;
        }

        
        for (int i = 0; i < dotPos.Count; i++)
        {
            Vector2Int currentPos = dotPos[i];

            //Get neighbors
            List<Vector2Int> adjecontDots = new List<Vector2Int>();
            List<Vector2Int> diagnolDots =  new List<Vector2Int>();

            for (int j = 0; j < dotPos.Count; j++)
            {
                if (i == j)
                    continue;

                //Find neighbors
                float distance = Vector2.Distance(currentPos, dotPos[j]);
                if(distance < 1.5f)
                    diagnolDots.Add(dotPos[j]);
                else if (distance <= 1f)
                    adjecontDots.Add(dotPos[j]);
            }

            //Check for empty
            if (diagnolDots.Count == 0 && adjecontDots.Count == 0)
            {
                Debug.Log("Error in making piece. Illegal dot placement.");
                return;
            }

            for (int k = 0; k < adjecontDots.Count; k++)
            {
                Vector2Int[] diagnolToCheck = ToolMath.PerpendicularVectors(adjecontDots[k] - currentPos);

                foreach (Vector2Int item in diagnolToCheck)
                {
                    if (diagnolDots.Contains(item))
                        diagnolDots.Remove(item);
                }
            }

            foreach (Vector2Int item in adjecontDots)
            {
                if (!PieceData.connectionsMade.HaveElement(currentPos, item))
                    PieceData.connectionsMade.AddElement(currentPos, item);
            }
            foreach (Vector2Int item in diagnolDots)
            {
                if (!PieceData.connectionsMade.HaveElement(currentPos, item))
                    PieceData.connectionsMade.AddElement(currentPos, item);
            }
        }

        base.Construct(targetSize);
    }

    public List<Vector2Int> GetDotCellPositions()
    {
        if (gridData == null || gridData.dotDictionary == null)
            return new List<Vector2Int>();

        List<Vector2Int> worldPositions = new List<Vector2Int>();
        foreach (var localPos in gridData.dotDictionary.Keys)
        {
            worldPositions.Add(localPos + gridData.gridPosRef); // Convert to world position
        }

        return worldPositions;
    }
}
