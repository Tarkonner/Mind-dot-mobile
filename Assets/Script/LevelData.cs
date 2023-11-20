using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string test;

    public int levelIndex;

    //Board
    public bool[,] placementLayer;
    public Cell[,] levelLayer;

    
    public Piece[] levelsPieces;
       
}
