using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Refences")]
    [SerializeField] Board board;
    [SerializeField] PieceMaker pieceHolder;
    [SerializeField] GoalMaker goalMaker;
    [SerializeField] LevelText levelText;

    [Header("Goals")]
    [SerializeField] private GameObject goalHolder;
    [SerializeField] private LevelSO[] levels;
    private List<IGoal> allGoals = new List<IGoal>();

    [Header("Testing")]
    [SerializeField] bool loadTestlevel = false;
    [SerializeField] private LevelSO testLevel;

    public int targetLevel { get; private set; } = 0;

    //Events
    public delegate void OnLoadLevel();
    public static event OnLoadLevel onLoadLevel;

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

        //Level index
        levelText.LevelIndex(targetLevel);
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
        onLoadLevel?.Invoke();

        //Clear old
        allGoals.Clear();

        board.LoadLevel(targetLevel); //Uses info from both board & pieces, so piece dots don't get loadet in
        goalMaker.MakeGoals(targetLevel);
        pieceHolder.MakePieces(targetLevel.levelPieces);
    }

    public void GoalProgression()
    {
        //See how many goals
        if(allGoals.Count == 0)
        {
            for (int i = 0; i < goalMaker.holder.childCount; i++)
            {
                IGoal check = goalMaker.holder.GetChild(i).GetComponent<IGoal>();

                if (check != null)
                {
                    allGoals.Add(check);
                }
            }
        }

        int completedGoals = 0;
        foreach (var child in allGoals)
        {
            if (child.CheckFulfilment(board))
            {
                completedGoals++;
            }
        }
        if (completedGoals == allGoals.Count)
        {
            //Tell test levels
#if (UNITY_EDITOR)
            if(loadTestlevel)
            {
                Debug.Log("Complete level");
                return;
            }
#endif
            targetLevel++;

            if (targetLevel == levels.Length)
                Debug.Log("All levels complete");
            else
            {
                Debug.Log("Level Complete");
                LoadLevel(levels[targetLevel]);
                levelText.LevelIndex(targetLevel);
            }
        }
    }
}