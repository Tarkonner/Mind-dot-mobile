using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelConverter
{
    private string version;
    public bool SaveLevel(string title, List<PieceElement> pEs, List<CellElement> board, Vector2 boardSize, List<GridElement> sGEs, LevelPlaceGoal[] lPGs)
    {
        bool success = true;

        try
        {
            LevelPiece[] pieces = new LevelPiece[pEs.Count];
            for (int i = 0; i < pEs.Count; i++)
            {
                pieces[i] = new LevelPiece(pEs[i]);
            }

            LevelShapeGoal[] levelShapeGoals = new LevelShapeGoal[sGEs.Count];
            for (int i = 0; i < sGEs.Count; i++)
            {
                List<Vector2Int> positions = sGEs[i].dotDictionary.Keys.ToList();
                List<DotElement> types = sGEs[i].dotDictionary.Values.ToList();

                levelShapeGoals[i] = new LevelShapeGoal(positions, types);
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

            LevelSO levelObject = new LevelSO(version, uniqueName, new LevelBoard(board, boardSize), pieces, levelShapeGoals, lPGs);

            AssetDatabase.CreateAsset(levelObject, uniquePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }
        catch (System.Exception)
        {
            return false;
        }
        
        return success;
    }

    public void LoadLevel(LevelSO levelObject)
    {

    }

}
