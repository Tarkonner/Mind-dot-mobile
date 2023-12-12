using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create Level", fileName = "Level", order = 0)]
public class LevelSO : ScriptableObject
{
    private string version;
    public string levelTitle;

    [SerializeField] public LevelBoard levelGrid;

    [SerializeField]public LevelPiece[] levelPieces;

    [SerializeField] public LevelShapeGoal[] levelShapeGoals;

    [SerializeField] public LevelPlaceGoal[] levelPlaceGoals;

    public LevelSO() { }
#nullable enable
    public LevelSO(string? version, string? levelTitle, LevelBoard levelGrid, 
        LevelPiece[] levelPieces, LevelShapeGoal[] levelGoals, LevelPlaceGoal[] levelPlaceGoals)
    {
        if (version != null) { this.version = version; }
        this.levelTitle = levelTitle;
        if (levelTitle != null) { this.name = levelTitle; }        
        this.levelGrid = levelGrid;
        this.levelPieces = levelPieces;
        this.levelShapeGoals = levelGoals;
        this.levelPlaceGoals = levelPlaceGoals;
    }
    private void Init(string? version, string? levelTitle, LevelBoard levelGrid,
        LevelPiece[] levelPieces, LevelShapeGoal[] levelGoals, LevelPlaceGoal[] levelPlaceGoals)
    {
        if (version != null) { this.version = version; }
        this.levelTitle = levelTitle;
        if (levelTitle != null) { this.name = levelTitle; }
        this.levelGrid = levelGrid;
        this.levelPieces = levelPieces;
        this.levelShapeGoals = levelGoals;
        this.levelPlaceGoals = levelPlaceGoals;
    }
    public static LevelSO CreateLevelSO(string? version, string? levelTitle, LevelBoard levelGrid,
        LevelPiece[] levelPieces, LevelShapeGoal[] levelGoals, LevelPlaceGoal[] levelPlaceGoals)
    {
        var levelSO = ScriptableObject.CreateInstance<LevelSO>();

        levelSO.Init(version, levelTitle, levelGrid, levelPieces, levelGoals, levelPlaceGoals);
        return levelSO;
    }
#nullable disable
}
