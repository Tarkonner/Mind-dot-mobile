using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{    
    public static Board Instance;

    [Header("Grid system")]
    private bool[,] placementLayer;
    private Cell[,] levelLayer;
    public Cell[,] grid;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private float gridSize;
    [SerializeField] private float spaceingBetweenCells = .2f;

    public Action onChange;

    [SerializeField] GameObject testDot;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadLevel(0);
    }

    public void LoadLevel(int level)
    {
        //Test level
        Vector2Int levelSize = new Vector2Int(4, 3);
        placementLayer = new bool[levelSize.x, levelSize.y];        
        levelLayer = new Cell[levelSize.x, levelSize.y];

        grid = new Cell[levelSize.x, levelSize.y];

        Vector2 targetOffset = new Vector2(
            (placementLayer.GetLength(0) / 2 * gridSize) + (placementLayer.GetLength(0) - 1) * spaceingBetweenCells + gridSize / 2,
            (placementLayer.GetLength(1) / 2 * gridSize) + (placementLayer.GetLength(1) - 1) * spaceingBetweenCells + gridSize / 2);

        for (int x = 0; x < placementLayer.GetLength(0); x++)
        {
            for (int y = 0; y < placementLayer.GetLength(1); y++)
            {
                //Placeholder level
                if (x == 0 && y == 0)
                    placementLayer[x, y] = false;
                else if (y == 2 && x == 3)
                    placementLayer[x, y] = false;
                else if (x == 2 && y == 2)
                {
                    placementLayer[x, y] = true;
                    Cell dotCell = new Cell();
                    dotCell.occupying = new Dot();
                    levelLayer[x, y] = dotCell;
                }
                else
                {
                    placementLayer[x, y] = true;
                    levelLayer[x, y] = new Cell();
                }

                //make grid
                if(placementLayer[x, y])
                {
                    GameObject cellSpawn = Instantiate(cellPrefab, transform);
                    cellSpawn.name = $"Cell: {x}, {y}";

                    cellSpawn.transform.localPosition = new Vector2(
                        x * gridSize + (x - 1) * spaceingBetweenCells - targetOffset.x / 2,
                        y * gridSize + (y - 1) * spaceingBetweenCells - targetOffset.y / 2);

                    Cell cell = cellSpawn.GetComponent<Cell>();
                    cell.occupying = levelLayer[x, y].occupying;
                    grid[x, y] = cell;
                    cell.gridPos = new Vector2Int(x, y);

                    //Spawn level
                    if (cell.occupying != null)
                        Instantiate(testDot, cellSpawn.transform);
                }
            }
        }
    }

    #region Board interaction
    public bool PlaceDot(Vector2Int coordinat, Dot dot)
    {
        return false;
    }

    public bool PlaceDots(Vector2Int coordinats, Piece piece)
    {
        return false;
    }

    public Dot GetDot(Vector2Int coordinat)
    {
        return null;
    }

    public List<Dot> GetDots(Vector2Int[] coordinats) 
    {
        return null;
    }

    public Piece GetPiece(Vector2Int coordinat)
    {
        return null;
    }
    #endregion
}
