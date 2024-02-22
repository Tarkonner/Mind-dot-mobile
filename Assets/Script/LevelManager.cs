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
    [SerializeField] bool loadTestlevel = false;
    [SerializeField] private LevelSO testLevel;

    private void Start()
    {
#if (UNITY_EDITOR)
        if(loadTestlevel && testLevel != null)
        {
            LoadLevel(testLevel);
            return;
        }
#endif
        LoadLevel(levels[0]);
    }

    public void LoadLevel(LevelSO targetLevel)
    {
        board.LoadLevel(targetLevel); //Uses info from both board & pieces, so piece dots don't get loadet in
        goalMaker.MakeGoals(targetLevel.levelShapeGoals);
        pieceHolder.MakePieces(targetLevel.levelPieces);
    }
}