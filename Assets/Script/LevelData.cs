using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ES3Serializable]
public class LevelData
{
    public string test;

    public int levelIndex;

    //Board
    public SerializableCell[,] levelCells;

    
    public Piece[] levelsPieces;
       
}
