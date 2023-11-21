using ES3Types;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] private string levelFolderPath;

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);

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
            Board.Instance.LoadLevel(save);
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

    public void LoadScene(int level)
    {
        SceneManager.LoadScene(level);            
    }


}