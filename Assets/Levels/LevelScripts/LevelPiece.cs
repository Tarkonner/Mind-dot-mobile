using SharedData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelPiece
{
    public Vector2[] dotPositions;
    public DotType[] dotTypes;
    public bool rotatable;
    public Vector2Int gridPosRef;
    public Vector2Int pieceSize;
    public int startRotation;

    public LevelPiece(PieceData pE)
    {
        int i = 0;
        dotTypes = new DotType[pE.dotDictionary.Count];
        dotPositions = new Vector2[pE.dotDictionary.Count];
        rotatable = pE.canRotate;
        startRotation = pE.startRotationIndex;
        foreach (KeyValuePair<Vector2Int, DotData> kVPair in pE.dotDictionary)
        {
            dotTypes[i] = kVPair.Value.dotType;
            dotPositions[i] = kVPair.Key;
            i++;
        }

        this.gridPosRef = pE.gridPosRef;

        pieceSize = pE.gridSize;
    }
}