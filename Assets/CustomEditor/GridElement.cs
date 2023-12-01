using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridElement : VisualElement
{
    protected Vector2Int gridSize;

    protected int imageSize = 30;
    protected int spaceing = 3;

    protected Sprite cellBackground;

    public GridElement()
    {
        style.flexDirection = FlexDirection.Row;
        style.flexWrap = Wrap.Wrap;
        style.justifyContent = Justify.SpaceAround;

        //Set sprite
        cellBackground = Resources.Load<Sprite>("Square");
    }

    public virtual void Construct()
    {
        style.width = gridSize.x * imageSize + gridSize.x * spaceing; // cellWidth is the width of each cell
        style.height = gridSize.y * imageSize + gridSize.y * spaceing; // cellHeight is the height of each cell

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Image targetImage = new Image();
                targetImage.sprite = cellBackground;
                targetImage.style.width = imageSize;
                targetImage.style.height = imageSize;

                //Flexbox
                targetImage.style.flexDirection = FlexDirection.Row;
                targetImage.style.justifyContent = Justify.Center;
                targetImage.style.alignItems = Align.Center;


                var cell = new VisualElement();
                cell.Add(targetImage);
                this.Add(targetImage);
            }
        }
    }

    public virtual void PlaceDot()
    {

    }

    public virtual void SetGridSize(Vector2Int size)
    {
        gridSize = size;
    }
}
