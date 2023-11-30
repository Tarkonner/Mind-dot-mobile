using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PieceElement : VisualElement
{
    //public List<Vector2Int> dotCoordinats = new List<Vector2Int>();
    public Dictionary<Vector2Int, DotElement> dots = new Dictionary<Vector2Int, DotElement>();

    private Vector2Int gridSize;

    private int imageSize = 30;
    private int spaceing = 3;

    public PieceElement() 
    {
        style.flexDirection = FlexDirection.Row;
        style.flexWrap = Wrap.Wrap;
        style.justifyContent = Justify.SpaceAround;
    }

    public void ConstructPiece()
    {
        style.width = gridSize.x * imageSize + gridSize.x * spaceing; // cellWidth is the width of each cell
        style.height = gridSize.y * imageSize + gridSize.y * spaceing; // cellHeight is the height of each cell

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Image targetImage = new Image();
                targetImage.sprite = Resources.Load<Sprite>("Square");
                targetImage.style.width = imageSize;
                targetImage.style.height = imageSize;

                //Flexbox
                targetImage.style.flexDirection = FlexDirection.Row;
                targetImage.style.justifyContent = Justify.Center;
                targetImage.style.alignItems = Align.Center;


                var cell = new VisualElement();
                cell.Add(targetImage);
                this.Add(targetImage);

                //Dot
                if(dots.ContainsKey(new Vector2Int(x, y)))
                {
                    DotElement spawnedDot = new DotElement(dots[new Vector2Int(x, y)].dotType);
                    targetImage.Add(spawnedDot);
                }
            }
        }
    }


    public void AddDot(Vector2Int coordinats, DotElement dot)
    {
        Vector2Int calculation = new Vector2Int(
            Mathf.Abs(coordinats.x), 
            Mathf.Abs(coordinats.y));

        dots.Add(calculation, dot);

        //Grid size
        if(calculation.x + 1 > gridSize.x)
            gridSize.x = calculation.x + 1;
        if(calculation.y + 1 > gridSize.y)
            gridSize.y = calculation.y + 1;
    }
}
