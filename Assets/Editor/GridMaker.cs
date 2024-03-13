using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridMaker 
{
    public static GridElement MakeGridElement(List<CellElement> targetElements)
    {
        if (targetElements.Count > 0)
        {
            //Goal or piece
            GridElement spawnedGrid = new GridElement();

            //Save siblings
            for (int i = 0; i < targetElements.Count; i++)
            {
                spawnedGrid.siblings.Add(targetElements[i]);
            }

            //Calculate position
            Vector2Int lowPoint = new Vector2Int(10, 10);
            Vector2Int highPoint = new Vector2Int(0, 0);
            for (int i = 0; i < targetElements.Count; i++)
            {
                //Low
                if (lowPoint.x > targetElements[i].cellData.gridCoordinates.x)
                    lowPoint.x = targetElements[i].cellData.gridCoordinates.x;
                if (lowPoint.y > targetElements[i].cellData.gridCoordinates.y)
                    lowPoint.y = targetElements[i].cellData.gridCoordinates.y;

                //High
                if (highPoint.x < targetElements[i].cellData.gridCoordinates.x)
                    highPoint.x = targetElements[i].cellData.gridCoordinates.x;
                if (highPoint.y < targetElements[i].cellData.gridCoordinates.y)
                    highPoint.y = targetElements[i].cellData.gridCoordinates.y;
            }
            spawnedGrid.gridData.gridPosRef = lowPoint;

            for (int i = 0; i < targetElements.Count; i++)
            {
                //Set refence point
                Vector2Int targetCoor = targetElements[i].cellData.gridCoordinates - new Vector2Int(lowPoint.x, lowPoint.y);

                spawnedGrid.AddDot(targetCoor, targetElements[i].dotRef);
            }

            spawnedGrid.Construct(new Vector2Int(spawnedGrid.gridData.gridSize.x, spawnedGrid.gridData.gridSize.y));

            return spawnedGrid;
        }
        else
        {
            Debug.Log("No cells chosen");
            return null;
        }
    }
}
