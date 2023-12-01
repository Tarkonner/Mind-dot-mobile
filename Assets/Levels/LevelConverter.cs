using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConverter
{
    private string version;
    public bool SaveLevel(List<PieceElement> pE, List<CellElement> board, Vector2 boardSize,  )
    {
        bool success = true;
        PieceElement[] pieces = new PieceElement[pE.Count];
        for (int i = 0; i < pE.Count; i++)
        {
            pieces[i] = pE[i];
        }

        LevelSO levelObject = new LevelSO(version, null, new LevelBoard(board,boardSize), pieces,);

        levelObject.levelGrid = new LevelBoard(board,boardSize);
        levelObject.levelPieces = new LevelPiece[pE.Count];

        return success;
    }

}
