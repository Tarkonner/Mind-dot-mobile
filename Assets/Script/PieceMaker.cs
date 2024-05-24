using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceMaker : ScaleAnimations
{
    [Header("Setup")]
    [SerializeField] float spaceBetweenPieces = 300;
    [SerializeField] float pieceSize = 270;

    [SerializeField] GameObject piecePrefab;
    [SerializeField] GameObject pieceBackground;

    [SerializeField] Transform holder;

    [Header("Animations")]
    [SerializeField] private float animateInTime = .5f;
    [SerializeField] private float animateOutTime = .5f;
    private List<GameObject> backgroundToAnimate = new List<GameObject>();   
    private List<GameObject> piecesToAnimate = new List<GameObject>();

    [Header("Scaling")]
    [SerializeField] float scalePerDot = .15f;

    public void MakePieces(LevelPiece[] levelsPieces)
    {
        //Remove old level
        if (holder.transform.childCount > 0)
        {
            ScaleOutLiniar(backgroundToAnimate, animateOutTime);
            ScaleOutLiniar(piecesToAnimate, animateOutTime);
            StartCoroutine(MakePieces(animateOutTime, levelsPieces));
        }
        else
            StartCoroutine(MakePieces(0, levelsPieces));
    }

    IEnumerator MakePieces(float waitTime, LevelPiece[] levelsPieces)
    {
        yield return new WaitForSeconds(waitTime + .05f);

        //Remove old
        for (int i = holder.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(holder.transform.GetChild(i).gameObject);
            backgroundToAnimate.Clear();
            piecesToAnimate.Clear();
        }

        //Calculation
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

            backgrundRec.sizeDelta = new Vector2(pieceSize, pieceSize);

            //Make piece
            GameObject spawnedPiece = Instantiate(piecePrefab, spawnedBackground.transform, false);
            Piece piece = spawnedPiece.GetComponent<Piece>();
            piece.LoadPiece(levelsPieces[i]);

            //Scale
            int pieceBiggetsSize = Mathf.Max(levelsPieces[i].pieceSize.x, levelsPieces[i].pieceSize.y);
            if (pieceBiggetsSize != 3)
            {
                int diff = 3 - pieceBiggetsSize;
                float calScale = piece.smallPieceSize + diff * scalePerDot;
                piece.smallPieceSize = calScale;
            }
            piece.ChangeState(Piece.pieceStats.small);

            //Animate
            piecesToAnimate.Add(spawnedPiece);
            backgroundToAnimate.Add(spawnedBackground);
        }

        //Animation
        ScaleInLiniar(backgroundToAnimate, animateInTime);
    }
}
