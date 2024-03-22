using SharedData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShapeGoalElement : GridElement
{
    public ShapeGoalElement() : base()
    {

    }

    public override void Construct(Vector2Int targetSize)
    {
        //Setup data
        gridData.gridSize = targetSize;

        images = new Image[gridData.gridSize.x, gridData.gridSize.y];

        style.width = gridData.gridSize.x * imageSize + gridData.gridSize.x; // cellWidth is the width of each cell
        style.height = gridData.gridSize.y * imageSize + gridData.gridSize.y; // cellHeight is the height of each cell

        for (int y = 0; y < gridData.gridSize.y; y++)
        {
            for (int x = 0; x < gridData.gridSize.x; x++)
            {
                Image targetImage = new ClickElement();
                images[x, y] = targetImage;

                targetImage.sprite = cellBackground;
                targetImage.style.width = imageSize;
                targetImage.style.height = imageSize;

                //Flexbox
                targetImage.style.flexDirection = FlexDirection.Row;
                targetImage.style.justifyContent = Justify.Center;
                targetImage.style.alignItems = Align.Center;

                //Add cell
                var cell = new VisualElement();
                cell.Add(targetImage);
                this.Add(cell);


                //Place dot
                if (gridData.dotDictionary.ContainsKey(new Vector2Int(x, y)))
                {
                    DotElement spawendDot = new DotElement(gridData.dotDictionary[new Vector2Int(x, y)].dotType);
                    gridData.dotDictionary[new Vector2Int(x, y)] = spawendDot.DotData;
                    images[x, y].Add(spawendDot);
                    ((ClickElement)targetImage).holdingDot = spawendDot;
                }
            }
        }
    }
}
