using ES3Types;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Refences")]
    [SerializeField] private Board board;
    [SerializeField] private GameUI gameUI;


    private void Awake()
    {
        ES3.Init();
    }


    private void Start()
    {
        SaveLevel();
    }

    public void LoadLevel(int level)
    {
        LevelData save = ES3.Load<LevelData>(level.ToString());

        if (save != null)
        {
            board.LoadLevel(save);
            gameUI.LoadLevel(save);
        }
        else
            Debug.LogError($"Tried to load: [{level}], but was not found");
    }

    public void SaveLevel()
    {
        //Test level
        LevelData level = new LevelData();
        level.test = "testing";
        level.levelIndex = 0;

        string key = level.levelIndex.ToString();

        #region Test Level
        Vector2Int levelSize = new Vector2Int(5, 3);
        SerializableCell[,] grid = new SerializableCell[levelSize.x, levelSize.y];
        for (int x = 0; x < levelSize.x; x++)
        {
            for (int y = 0; y < levelSize.y; y++)
            {
                //Placeholder level
                if (x == 0 && y == 0)
                {
                    grid[x, y] = new SerializableCell();
                }
                else if (y == 2 && x == 3)
                    grid[x, y] = new SerializableCell();
                else if (x == 1 && y == 1)
                {
                    Cell dotCell = new Cell();
                    Dot d = new Dot();
                    d.dotType = DotType.Blue;
                    dotCell.occupying = d;
                    grid[x, y] = new SerializableCell(dotCell);
                }
                else
                {
                    grid[x, y] = new SerializableCell(new Cell());
                }
            }
        }

        //Test Pieces
        SerializablePiece[] savePieces = new SerializablePiece[2];
        //P1
        Vector2[] dotCoordinats = new Vector2[]
            {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0)};
        Dot[] pieceDots = new Dot[3];
        for (int i = 0; i < pieceDots.Length; i++)
            pieceDots[i] = new Dot();
        pieceDots[0].dotType = DotType.Blue;
        pieceDots[1].dotType = DotType.Red;
        pieceDots[2].dotType = DotType.Blue;

        Piece piece = new Piece();
        piece.gridPosArray = dotCoordinats;
        piece.dotsArray = pieceDots;
        SerializablePiece sPiece = SaveConverter.ConvertToSerializablePiece(piece);
        savePieces[0] = sPiece;

        //P2
        dotCoordinats = new Vector2[]
        {new Vector2(0, 0), new Vector2(0, -1), new Vector2(-1, 0)};
        pieceDots = new Dot[3];
        for (int i = 0; i < pieceDots.Length; i++)
            pieceDots[i] = new Dot();
        pieceDots[0].dotType = DotType.Blue;
        pieceDots[1].dotType = DotType.Yellow;
        pieceDots[2].dotType = DotType.Red;

        piece = new Piece();
        piece.gridPosArray = dotCoordinats;
        piece.dotsArray = pieceDots;
        sPiece = SaveConverter.ConvertToSerializablePiece(piece);
        savePieces[1] = sPiece;

        level.levelsPieces = savePieces;
        #endregion

        //Save grid
        level.levelCells = grid;

        //Gate
        if (ES3.KeyExists(key))
        {
            Debug.Log("Need new key for level");
            return;
        }
        else
        {
            Debug.Log("New file added");
            ES3.Save(key, level);
        }

    }
}