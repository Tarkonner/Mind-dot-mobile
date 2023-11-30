using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPiece
{
    public Vector2[] dotPositions;
    public DotType[] dotTypes;

    public LevelPiece(Piece piece)
    {
        dotPositions = piece.gridPosArray;
        dotTypes = new DotType[piece.dotsArray.Length];
        for (int i = 0; i < piece.dotsArray.Length;)
        {
            dotTypes[i] = piece.dotsArray[i].dotType;
        }
    }
}