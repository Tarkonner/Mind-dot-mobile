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
        offset.y = 0;
        if (levelsPieces.Length == 1)
            offset.x = 0;
        else
        {
            offset.x = -spaceBetweenPieces * (levelsPieces.Length / 2);
            if (levelsPieces.Length % 2 == 0)
                offset.x += spaceBetweenPieces / 2;
        }

        for (int i = 0; i < levelsPieces.Length; i++)
        {
            Vector2 calPosition = new Vector2(i * spaceBetweenPieces + offset.x, offset.y);

            //Make Background
            GameObject spawnedBackground = Instantiate(pieceBackground, holder);
            RectTransform backgrundRec = spawnedBackground.GetComponent<RectTransform>();
            backgrundRec.localPosition = calPosition;
            int pieceBiggetsSize = Mathf.Max(levelsPieces[i].pieceSize.x, levelsPieces[i].pieceSize.y);
            backgrundRec.sizeDelta = new Vector2(90 * pieceBiggetsSize, 90 * pieceBiggetsSize);

            //Make piece
            GameObject spawnedPiece = Instantiate(piecePrefab, spawnedBackground.transform, false);    
            Piece piece = spawnedPiece.GetComponent<Piece>();
            piece.LoadPiece(levelsPieces[i]);
            piece.ChangeState(Piece.pieceStats.small);
        }
    }
}
