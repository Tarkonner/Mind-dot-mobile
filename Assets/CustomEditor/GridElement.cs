using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GridElement : VisualElement
{
    public Dictionary<Vector2Int, DotElement> dotDictionary = new Dictionary<Vector2Int, DotElement>();

    protected Vector2Int gridSize;
    protected Image[,] images = null;

    protected int imageSize = 30;
    protected int spacing = 3;

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
        images = new Image[gridSize.x, gridSize.y];

        style.width = gridSize.x * imageSize + gridSize.x * spacing; // cellWidth is the width of each cell
        style.height = gridSize.y * imageSize + gridSize.y * spacing; // cellHeight is the height of each cell

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Image targetImage = new Image();
                images[x, y] = targetImage;

                targetImage.sprite = cellBackground;
                targetImage.style.width = imageSize;
                targetImage.style.height = imageSize;

                //Flexbox
                targetImage.style.flexDirection = FlexDirection.Row;
                targetImage.style.justifyContent = Justify.Center;
                targetImage.style.alignItems = Align.Center;

                //Add scell
                var cell = new VisualElement();
                cell.Add(targetImage);
                this.Add(targetImage);

                if(dotDictionary.ContainsKey(new Vector2Int(x, y)))
                {
                    DotElement spawendDot = new DotElement(dotDictionary[new Vector2Int(x, y)].dotType);
                    images[x, y].Add(spawendDot);
                }
            }
        }
    }

    public virtual void AddDot(Vector2Int coordinat, DotElement dot)
    {
        dotDictionary.Add(coordinat, dot);
    }

    public virtual bool PlaceDot(Vector2Int coordinats)
    {
        if(dotDictionary.ContainsKey(coordinats))
        {
            DotElement targetDot = dotDictionary[coordinats];
            images[coordinats.x, coordinats.y].Add(targetDot);

            return true;
        }
        else
            return false;
    }

    public virtual void SetGridSize(Vector2Int size)
    {
        gridSize = size;
    }
}
