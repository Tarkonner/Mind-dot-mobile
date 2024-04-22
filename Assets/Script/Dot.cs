using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dot : MonoBehaviour, IOccupying
{
    [Header("Colors")]
    public Color redColor;
    public Color yellowColor;
    public Color blueColor;

    public DotType dotType;


    public Piece parentPiece;
    public Vector2 relativePosition;

    public Cell cell;


    public void Setup(DotType targetType, Piece parentPiece = null)
    {
        dotType = targetType;

        //Is part of a piece
        this.parentPiece = parentPiece;

        //Set color
        Image renderer = GetComponent<Image>();
        
        switch (dotType)
        {
            case DotType.Red:
                renderer.color = redColor;
                break;
            case DotType.Blue:
                renderer.color = blueColor;
                break;
            case DotType.Yellow:
                renderer.color = yellowColor;
                break;
        }
    }
}
