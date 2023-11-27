using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Holders")]
    [SerializeField] GameObject goalHolder;
    [SerializeField] GameObject pieceHolder;
    [SerializeField] GameObject pieceBackground;
    [SerializeField] float paddingBetweenPieces = 300;

    [SerializeField] GameObject[] pieces;
    [SerializeField] GameObject[] goals;

    void Start()
    {
        //Goal
        for (int i = 0; i < goals.Length; i++)
        {
            GameObject spawn = Instantiate(goals[i], transform);
            spawn.transform.SetParent(goalHolder.transform,false);
        }

        //Pieces
        float offset = 0;
        if(pieces.Length > 1)
        {
            offset = paddingBetweenPieces * (pieces.Length - 1) / 2;
        }
        for (int i = 0;i < pieces.Length; i++)
        {
            Vector2 targetPos = new Vector2(i * paddingBetweenPieces - offset, 0);

            GameObject background = Instantiate(pieceBackground, pieceHolder.transform);
            background.GetComponent<RectTransform>().anchoredPosition = targetPos;
            GameObject spawn = Instantiate(pieces[i], background.transform);
            spawn.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }
}
