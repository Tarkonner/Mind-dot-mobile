using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create Level", fileName = "Level", order = 0)]
public class LevelSO : ScriptableObject
{
    private string version;
    public string levelTitle;

    [SerializeField] public LevelBoard levelGrid;

    [SerializeField] public LevelPiece[] levelPieces;

    [SerializeField] public LevelShapeGoal[] levelShapeGoals;

    [SerializeField] public LevelPlaceGoal[] levelPlaceGoals;

    [SerializeField, Range(0, 3)] public int[] pieceStartRotation;

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

        // Placeholder for ramdom rotation
        pieceStartRotation = new int[levelPieces.Length];
        for (int i = 0; i < pieceStartRotation.Length; i++)
        {
            int rnd = Random.Range(0, 4);
            levelPieces[i].startRotation = rnd;
            pieceStartRotation[i] = rnd;
        }
    }
    public static LevelSO CreateLevelSO(string? version, string? levelTitle, LevelBoard levelGrid,
        LevelPiece[] levelPieces, LevelShapeGoal[] levelGoals, LevelPlaceGoal[] levelPlaceGoals)
    {
        var levelSO = ScriptableObject.CreateInstance<LevelSO>();

        levelSO.Init(version, levelTitle, levelGrid, levelPieces, levelGoals, levelPlaceGoals);

        return levelSO;
    }
#nullable disable

    private void OnValidate()
    {
        //Rotation
        for (int i = 0; i < pieceStartRotation.Length; i++)
        {
            if (levelPieces[i].rotatable)
                levelPieces[i].startRotation = pieceStartRotation[i];
        }
    }
}
