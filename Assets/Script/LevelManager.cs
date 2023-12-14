using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Refences")]
    [SerializeField] private Board board;
    [SerializeField] private GameUI gameUI;

    [SerializeField] private LevelSO[] levels;

    [Header("Testing")]
    [SerializeField] private LevelSO TargetLevel;

    private void Start()
    {
        if(TargetLevel != null)
        {
            board.LoadLevel(TargetLevel);
        }
    }

    public void LoadLevel()
    {
    }
}