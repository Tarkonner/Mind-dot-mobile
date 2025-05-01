using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Create Level", fileName = "Level", order = 0)]
public class LevelSO : ScriptableObject
{
    public int version = 0;
    [Header("Level info")]
    public string levelTitle;
    public LevelBoard levelGrid;
    public LevelPiece[] levelPieces;
    public LevelShapeGoal[] levelShapeGoals;
    public LevelPlaceGoal[] levelPlaceGoals;

    [Header("Meta data")]
    public int[] piecesPlacementOptions;
    public int totalPlacementOptions;


    public LevelSO() { }
#nullable enable
    private void Init(string? levelTitle, LevelBoard levelGrid,
        LevelPiece[] levelPieces, LevelShapeGoal[] levelGoals, LevelPlaceGoal[] levelPlaceGoals,
        int[] piecesPlacementOptions)
    {
        this.levelTitle = levelTitle;
        if (levelTitle != null) 
            this.name = levelTitle;
        this.levelGrid = levelGrid;
        this.levelPieces = levelPieces;
        this.levelShapeGoals = levelGoals;
        this.levelPlaceGoals = levelPlaceGoals;

        //Meta data
        CalLevelTotalPossibleChoices(piecesPlacementOptions);
    }

    public void LevelOverride(LevelBoard levelGrid, LevelPiece[] levelPieces, LevelShapeGoal[] levelGoals, LevelPlaceGoal[] levelPlaceGoals,
        int[] piecesPlacementOptions)
    {
#if (UNITY_EDITOR)

        this.levelGrid = levelGrid;
        this.levelPieces = levelPieces;
        this.levelShapeGoals = levelGoals;
        this.levelPlaceGoals = levelPlaceGoals;

        CalLevelTotalPossibleChoices(piecesPlacementOptions);

        UnityEditor.EditorUtility.SetDirty(this);

#endif
    }

    public static LevelSO CreateLevelSO(string? levelTitle, LevelBoard levelGrid,
        LevelPiece[] levelPieces, LevelShapeGoal[] levelGoals, LevelPlaceGoal[] levelPlaceGoals,
        int[] piecesPlacementOptions)
    {
        var levelSO = ScriptableObject.CreateInstance<LevelSO>();

        levelSO.Init(levelTitle, levelGrid, levelPieces, levelGoals, levelPlaceGoals, piecesPlacementOptions);

        return levelSO;
    }

    void CalLevelTotalPossibleChoices(int[] piecePlacementInfo)
    {
        piecesPlacementOptions = piecePlacementInfo;
        for (int i = 0; i < piecesPlacementOptions.Length; i++)
            totalPlacementOptions += piecesPlacementOptions[i];
    }
#nullable disable
}
