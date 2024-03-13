using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridMaker 
{
    public static GridElement MakeGridElement(List<CellElement> targetElements, GridElement gridType)
    {
        //Save siblings
        for (int i = 0; i < targetElements.Count; i++)
        {
            gridType.siblings.Add(targetElements[i]);
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
        gridType.gridData.gridPosRef = lowPoint;

        for (int i = 0; i < targetElements.Count; i++)
        {
            //Set refence point
            Vector2Int targetCoor = targetElements[i].cellData.gridCoordinates - new Vector2Int(lowPoint.x, lowPoint.y);

            gridType.AddDot(targetCoor, targetElements[i].dotRef);
        }

        gridType.Construct(new Vector2Int(gridType.gridData.gridSize.x, gridType.gridData.gridSize.y));

        return gridType;
    }


}
