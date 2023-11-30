using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PieceElement : VisualElement
{
    //public List<Vector2Int> dotCoordinats = new List<Vector2Int>();
    public Dictionary<Vector2Int, DotElement> dots = new Dictionary<Vector2Int, DotElement>();

    private Vector2Int gridSize;

    public void MakePiece()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Image targetImage = new Image();
                targetImage.sprite = Resources.Load<Sprite>("Square");
                targetImage.style.width = 30;
                targetImage.style.height = 30;

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
                    
                }
            }
        }
    }


    public void AddDot(Vector2Int coordinats, DotElement dot)
    {
        dots.Add(coordinats, dot);

        //Grid size
        if(Mathf.Abs(coordinats.x) + 1 > gridSize.x)
            gridSize.x = Mathf.Abs(coordinats.x) + 1;
        if(Mathf.Abs(coordinats.y) + 1 > gridSize.y)
            gridSize.y = Mathf.Abs(coordinats.y) + 1;
    }
}
