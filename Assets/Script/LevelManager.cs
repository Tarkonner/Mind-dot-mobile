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

    [Header("Goals")]
    [SerializeField] private GameObject goalHolder;
    [SerializeField] private Color uncompleteGoalColor;
    [SerializeField] private Color completedGoalColor;
    [SerializeField] private LevelSO[] levels;
    private List<IGoal> allGoals = new List<IGoal>();

    [Header("Testing")]
    [SerializeField] bool loadTestlevel = false;
    [SerializeField] private LevelSO testLevel;

    public int targetLevel = 0;

    private void Start()
    {
        //Load level
#if (UNITY_EDITOR)
        if(loadTestlevel && testLevel != null)
        {
            LoadLevel(testLevel);
            return;
        }
#endif
        //Load first level
        LoadLevel(levels[targetLevel]);
    }

    private void OnEnable()
    {
        InputSystem.onDotChange += GoalProgression;
    }
    private void OnDisable()
    {
        InputSystem.onDotChange -= GoalProgression;
    }

    public void LoadLevel(LevelSO targetLevel)
    {
        board.LoadLevel(targetLevel); //Uses info from both board & pieces, so piece dots don't get loadet in
        goalMaker.MakeGoals(targetLevel);
        pieceHolder.MakePieces(targetLevel.levelPieces);

        for (int i = 0; i < goalMaker.holder.childCount; i++)
        {
            IGoal check = goalMaker.holder.GetChild(i).GetComponent<CountPlacementGoals>();

            if (check != null)
                allGoals.Add(check);
        }
    }

    public void GoalProgression()
    {
        int completedGoals = 0;
        foreach (var child in allGoals)
        {
            if (child.CheckFulfilment(board))
            {
                completedGoals++;
            }
        }
        if (completedGoals >= allGoals.Count)
        {
            //Tell test levels
#if (UNITY_EDITOR)
            if(testLevel)
            {
                Debug.Log("Complete level");
            }
#endif
            targetLevel++;

            if (targetLevel == levels.Length)
                Debug.Log("All levels complete");
            else
            {
                Debug.Log("Level Complete");
                LoadLevel(levels[targetLevel]);
            }
        }
    }
}