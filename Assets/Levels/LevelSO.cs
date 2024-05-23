using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Create Level", fileName = "Level", order = 0)]
public class LevelSO : ScriptableObject
{
    public int version = 0;
    public string levelTitle;

    [SerializeField] public LevelBoard levelGrid;
    [SerializeField] public LevelPiece[] levelPieces;
    [SerializeField] public LevelShapeGoal[] levelShapeGoals;
    [SerializeField] public LevelPlaceGoal[] levelPlaceGoals;

    public LevelSO() { }
#nullable enable
    private void Init(string? levelTitle, LevelBoard levelGrid,
        LevelPiece[] levelPieces, LevelShapeGoal[] levelGoals, LevelPlaceGoal[] levelPlaceGoals)
    {
        this.levelTitle = levelTitle;
        if (levelTitle != null) 
            this.name = levelTitle;
        this.levelGrid = levelGrid;
        this.levelPieces = levelPieces;
        this.levelShapeGoals = levelGoals;
        this.levelPlaceGoals = levelPlaceGoals;
    }

    public void CreateVariation(LevelBoard levelGrid, LevelPiece[] levelPieces, LevelShapeGoal[] levelGoals, LevelPlaceGoal[] levelPlaceGoals)
    {
#if (UNITY_EDITOR)
        version++;
        string targetName = levelTitle + "v" + version;
        LevelSO levelObject = LevelSO.CreateLevelSO(targetName, levelGrid, levelPieces, levelGoals, levelPlaceGoals);

        string uniquePath = AssetDatabase.GenerateUniqueAssetPath($"Assets/Levels/{targetName}.asset");
        string uniqueName = uniquePath.Replace("Assets/Levels/", "");
        uniqueName = uniqueName.Replace(".asset", "");

        AssetDatabase.CreateAsset(levelObject, uniquePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }

    public static LevelSO CreateLevelSO(string? levelTitle, LevelBoard levelGrid,
        LevelPiece[] levelPieces, LevelShapeGoal[] levelGoals, LevelPlaceGoal[] levelPlaceGoals)
    {
        var levelSO = ScriptableObject.CreateInstance<LevelSO>();

        levelSO.Init(levelTitle, levelGrid, levelPieces, levelGoals, levelPlaceGoals);

        return levelSO;
    }
#nullable disable
}
