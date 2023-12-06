using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSO : ScriptableObject
{
    private string version;
    public string levelTitle;

    public LevelBoard levelGrid;

    public LevelPiece[] levelPieces;

    public LevelShapeGoal[] levelGoals;

    public LevelPlaceGoal[] levelPlaceGoals;

    public LevelSO() { }
#nullable enable
    public LevelSO(string? version, string? levelTitle, LevelBoard levelGrid, 
        LevelPiece[] levelPieces, LevelShapeGoal[] levelGoals, LevelPlaceGoal[] levelPlaceGoals)
    {
        if (version != null) { this.version = version; }
        this.levelTitle = levelTitle;
        this.name = levelTitle;
        this.levelGrid = levelGrid;
        this.levelPieces = levelPieces;
        this.levelGoals = levelGoals;
        this.levelPlaceGoals = levelPlaceGoals;
    }
}
