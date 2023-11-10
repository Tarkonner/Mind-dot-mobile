using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceContainer : MonoBehaviour
{
    public List<Piece> pieces;
    public float pieceSpacing = 1;

    void AddPiece(Piece piece)
    {
        piece.gameObject.transform.parent = this.gameObject.transform;
        piece.gameObject.transform.localScale = (Vector3.one/3);
        piece.gameObject.transform.localPosition = new Vector3(pieces.Count*pieceSpacing,0f,0f);
        pieces.Add(piece);
    }
    void RemovePiece(int pieceIndex)
    {
        pieces.RemoveAt(pieceIndex);
    }
}
