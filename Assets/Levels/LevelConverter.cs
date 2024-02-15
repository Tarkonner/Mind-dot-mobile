using SharedData;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public static class LevelConverter
{
    private static string version = "0.1";
    public static bool SaveLevel(string title, List<PieceData> pEs, 
        List<CellData> board, Vector2 boardSize, List<GridData> sGEs, List<PlaceGoalData> pGEs)
    {
        bool success = true;
        LevelPiece[] pieces = new LevelPiece[pEs.Count];
        for (int i = 0; i < pEs.Count; i++)
        {
            pieces[i] = new LevelPiece(pEs[i]);
        }

        LevelShapeGoal[] levelShapeGoals = new LevelShapeGoal[sGEs.Count];
        for (int i = 0; i < sGEs.Count; i++)
        {
            levelShapeGoals[i] = new LevelShapeGoal(sGEs[i]);
        }

        LevelPlaceGoal[] lPGs = new LevelPlaceGoal[pGEs.Count];
        for (int i = 0; i < pGEs.Count; i++)
        {
            lPGs[i] = new LevelPlaceGoal(pGEs[i].cellData.gridCoordinates, pGEs[i].goalType);
        }
        string name;
        if (title == null || title == "")
        {
            name = "New Level";
        }
        else
        {
            name = title;
        }

        string uniquePath = AssetDatabase.GenerateUniqueAssetPath($"Assets/Levels/{name}.asset");
        string uniqueName = uniquePath.Replace("Assets/Levels/", "");
        uniqueName = uniqueName.Replace(".asset", "");
        LevelSO levelObject = LevelSO.CreateLevelSO(version, uniqueName, new LevelBoard(board, boardSize), pieces, levelShapeGoals, lPGs);

        AssetDatabase.CreateAsset(levelObject, uniquePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return success;
    }

    public static void LoadLevel(LevelSO levelObject)
    {

    }

}
