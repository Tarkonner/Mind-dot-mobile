using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]

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
    public LevelPiece(PieceElement pE)
    {
        int i = 0;
        dotTypes = new DotType[pE.dotDictionary.Count];
        dotPositions = new Vector2[pE.dotDictionary.Count];
        foreach (KeyValuePair<Vector2Int,DotElement> kVPair in pE.dotDictionary)
        {
            dotTypes[i] = kVPair.Value.dotType;
            dotPositions[i] = kVPair.Key;
            i++;
        }
    }
}