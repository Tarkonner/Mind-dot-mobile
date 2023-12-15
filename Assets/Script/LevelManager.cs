using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Refences")]
    [SerializeField] Board board;
    [SerializeField] PieceMaker pieceHolder;
    [SerializeField] GoalMaker goalMaker;

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
        goalMaker.MakeGoals(targetLevel.levelShapeGoals);
        pieceHolder.MakePieces(targetLevel.levelPieces);
    }
}