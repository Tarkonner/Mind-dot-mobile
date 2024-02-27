using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMaker : ScaleAnimations
{
    [SerializeField] float spaceBetweenPieces = 300;

    [SerializeField] GameObject piecePrefab;
    [SerializeField] GameObject pieceBackground;

    [SerializeField] Transform holder;

    [Header("Animations")]
    [SerializeField] private float animateInTime = .5f;
    [SerializeField] private float animateOutTime = .5f;
    private List<GameObject> piecesToAnimate = new List<GameObject>();

    public void MakePieces(LevelPiece[] levelsPieces)
    {
        //Remove old level
        if (holder.transform.childCount > 0)
        {
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
            int pieceBiggetsSize = Mathf.Max(levelsPieces[i].pieceSize.x, levelsPieces[i].pieceSize.y);
            backgrundRec.sizeDelta = new Vector2(90 * pieceBiggetsSize, 90 * pieceBiggetsSize);

            //Make piece
            GameObject spawnedPiece = Instantiate(piecePrefab, spawnedBackground.transform, false);
            Piece piece = spawnedPiece.GetComponent<Piece>();
            piece.LoadPiece(levelsPieces[i]);
            piece.ChangeState(Piece.pieceStats.small);

            //Animate
            piecesToAnimate.Add(spawnedBackground);
        }

        //Animation
        ScaleInLiniar(piecesToAnimate, animateInTime);
    }
}
