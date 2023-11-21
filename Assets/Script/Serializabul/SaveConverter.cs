using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveConverter 
{
    //Dot
    public static Dot ConvertToDot(SerializableDot seriDot)
    {
        Dot result = new Dot();
        result.dotType = seriDot.dotType;

        return result;
    }

    public static Dot ConvertToDot(SerializableDot seriDot, Piece targetPiece)
    {
        Dot result = new Dot();
        result.dotType = seriDot.dotType;

        result.parentPiece = targetPiece;

        return result;
    }

    public static SerializableDot ConvertToSerializableDot(Dot dot)
    {
        SerializableDot serializableDot = new SerializableDot();
        serializableDot.dotType = dot.dotType;
        return serializableDot;
    }

    //Pieces
    public static SerializablePiece ConvertToSerializablePiece(Piece targetPiece)
    {
        SerializablePiece piece = new SerializablePiece();
        //Position
        piece.gridPosArray = targetPiece.gridPosArray;
        //Dots
        SerializableDot[] saveDots = new SerializableDot[targetPiece.dotsArray.Length];
        for (int i = 0; i < targetPiece.dotsArray.Length; i++)
            saveDots[i] = SaveConverter.ConvertToSerializableDot(targetPiece.dotsArray[i]);
        piece.dotsArray = saveDots;

        return piece;
    }

    //Cell

}
