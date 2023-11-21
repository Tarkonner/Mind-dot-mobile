using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ES3Serializable]
public class SerializablePiece
{
    //Dots
    public Vector2[] gridPosArray;
    public SerializableDot[] dotsArray;

    //Hint system
    public int soultionRotation;
    public Vector2Int soultionPosition;
}
