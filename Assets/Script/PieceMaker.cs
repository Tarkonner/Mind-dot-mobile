using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMaker : MonoBehaviour
{
    [SerializeField] float spaceBetweenPieces = 300;

    [SerializeField] GameObject piecePrefab;
    [SerializeField] GameObject pieceBackground;

    [SerializeField] Transform holder;

    List<Piece> pieceList;

    public void RemoveAllPieces()
    {
        for (int i = pieceList.Count - 1; i >= 0; i--)
            Destroy(pieceList[i].gameObject);
    }

    public void MakePieces(LevelPiece[] levelsPieces)
    {
        Vector2 offset;
        offset.y = -100;
        if (levelsPieces.Length == 1)
            offset.x = 0;
        else
        {
            offset.x = -spaceBetweenPieces * levelsPieces.Length / 2;
            if (levelsPieces.Length % 2 == 1)
                offset.x -= spaceBetweenPieces / 2;
        }
        for (int i = 0; i < levelsPieces.Length; i++)
        {
            Vector2 calPosition = new Vector2(offset.x + i * spaceBetweenPieces, offset.y);

            ////Make Background
            //GameObject spawnedBackground = Instantiate(pieceBackground, holder);
            //RectTransform backgrundRec = spawnedBackground.GetComponent<RectTransform>();
            //backgrundRec.localPosition = calPosition;
            //backgrundRec.localScale = new Vector2(3, 3);

            //Make piece
            GameObject spawnedPiece = Instantiate(piecePrefab, holder);
            RectTransform pieceRec = spawnedPiece.GetComponent<RectTransform>();
            pieceRec.localPosition = calPosition;
            pieceRec.localScale = new Vector2(0.5f, 0.5f);
            Piece piece = spawnedPiece.GetComponent<Piece>();
            piece.LoadPiece(levelsPieces[i]);

        }
    }
}
