using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Vector2Int[] posArray;
    public Dot[] dotsArray;
    public Vector2 povitPoint;

    private LineRenderer[] connections;

    private bool rotatebul;

    public void LoadPiece()
    {

    }

    public void Rotate()
    {
        if (!rotatebul)
            return;
    }

    public void Place(Vector2Int coordinats)
    {

    }

    public void Lift()
    {

    }
}
