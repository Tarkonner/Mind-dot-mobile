using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dot : MonoBehaviour, IOccupying
{
    public DotType dotType;

    private bool isConnected;

    public Piece parentPiece;

    //Consider moving Grid Position storage into here.

    public bool IsConnected { get => isConnected; set => isConnected = value; }

    public void Setup(DotType targetType, Piece parentPiece = null)
    {
        //Is part of a piece
        this.parentPiece = parentPiece;

        //Set color
        Image renderer = GetComponent<Image>();
        
        switch (targetType)
        {
            case DotType.Red:
                renderer.color = Color.red;
                break;
            case DotType.Blue:
                renderer.color = Color.blue;
                break;
            case DotType.Yellow:
                renderer.color = Color.yellow;
                break;
        }
    }

    public Dot ConvertToDot(SerializableDot seriDot)
    {
        this.dotType = seriDot.dotType;
        Setup(this.dotType);

        return this;
    }

    public Dot ConvertToDot(SerializableDot seriDot, Piece targetPiece)
    {
        this.dotType = seriDot.dotType;
        Setup(this.dotType);

        this.parentPiece = targetPiece; 
        
        return this;
    }

    public SerializableDot ConvertToSerializableDot(Dot dot)
    {
        SerializableDot serializableDot = new SerializableDot();
        serializableDot.dotType = dot.dotType;
        return serializableDot;
    }
}
