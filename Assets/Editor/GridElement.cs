using SharedData;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GridElement : VisualElement
{
    public GridData gridData;

    protected Image[,] images = null;

    protected int imageSize = 30;
    protected int spacing = 3;

    public List<CellElement> siblings = new List<CellElement>();

    protected Sprite cellBackground;

    protected LevelEditor editor;


    public GridElement(LevelEditor editor)
    {
        style.flexDirection = FlexDirection.Row;
        style.flexWrap = Wrap.Wrap;
        style.justifyContent = Justify.SpaceAround;

        //Set sprite
        cellBackground = Resources.Load<Sprite>("Square");

        // Add event handlers for mouse down and up events
        RegisterCallback<MouseDownEvent>(OnMouseDown);

        this.editor = editor;
    }

    public virtual void Construct()
    {
        images = new Image[gridData.gridSize.x, gridData.gridSize.y];

        style.width = gridData.gridSize.x * imageSize + gridData.gridSize.x * spacing; // cellWidth is the width of each cell
        style.height = gridData.gridSize.y * imageSize + gridData.gridSize.y * spacing; // cellHeight is the height of each cell

        for (int y = 0; y < gridData.gridSize.y; y++)
        {
            for (int x = 0; x < gridData.gridSize.x; x++)
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

                if (gridData.dotDictionary.ContainsKey(new Vector2Int(x, y)))
                {
                    DotElement spawendDot = new DotElement(gridData.dotDictionary[new Vector2Int(x, y)].dotType);
                    images[x, y].Add(spawendDot);
                }
            }
        }
    }

    public virtual void AddDot(Vector2Int coordinat, DotElement dot)
    {
        Vector2Int calculation = new Vector2Int(
            Mathf.Abs(coordinat.x),
            Mathf.Abs(coordinat.y));

        //Grid size
        if (calculation.x + 1 > gridData.gridSize.x)
            gridData.gridSize.x = calculation.x + 1;
        if (calculation.y + 1 > gridData.gridSize.y)
            gridData.gridSize.y = calculation.y + 1;

        gridData.dotDictionary.Add(coordinat, dot.DotData);
    }

    //Broke then change to shared variables
    //public virtual bool PlaceDot(Vector2Int coordinats)
    //{
    //    if (gridData.dotDictionary.ContainsKey(coordinats))
    //    {
    //        DotElement targetDot = gridData.dotDictionary[coordinats];
    //        images[coordinats.x, coordinats.y].Add(targetDot);

    //        return true;
    //    }
    //    else
    //        return false;
    //}

    public virtual void SetGridSize(Vector2Int size)
    {
        gridData.gridSize = size;
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        // Call the OnCellClicked method of the LevelEditor window
        editor.ContentManagement(this);
    }
}
