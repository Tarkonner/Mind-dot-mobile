using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dot : MonoBehaviour, IOccupying
{
    [Header("Colors & images")]
    [SerializeField] ColorBank colorBank;
    [SerializeField] Sprite dotSprite;
    [SerializeField] Sprite triColorDot;

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
                renderer.sprite = dotSprite;
                renderer.color = colorBank.redColor;
                break;
            case DotType.Blue:
                renderer.sprite = dotSprite;
                renderer.color = colorBank.blueColor;
                break;
            case DotType.Yellow:
                renderer.sprite = dotSprite;
                renderer.color = colorBank.yellowColor;
                break;
            case DotType.Null:
                renderer.sprite = triColorDot;
                renderer.color = Color.white;
                break;
        }
    }
}
