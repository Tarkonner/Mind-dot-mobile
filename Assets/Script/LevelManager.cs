using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Holders")]
    [SerializeField] GameObject goalHolder;
    [SerializeField] GameObject pieceBackground;
    [SerializeField] float paddingBetweenPieces = 300;

    [Header("Refences")]
    [SerializeField] Board board;
    [SerializeField] PieceMaker pieceHolder;


    [SerializeField] private LevelSO[] levels;

    [Header("Testing")]
    [SerializeField] private LevelSO testLevel;

    private void Start()
    {
        if(testLevel != null)
            LoadLevel(testLevel);
    }

    public void LoadLevel(LevelSO targetLevel)
    {
        board.LoadLevel(targetLevel);
        pieceHolder.MakePieces(targetLevel.levelPieces);
    }
}