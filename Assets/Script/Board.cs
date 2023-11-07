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
        placementLayer = new bool[4, 3];

        Vector2 targetOffset = new Vector2(
            (placementLayer.GetLength(0) / 2 * gridSize) + (placementLayer.GetLength(0) - 1) * spaceingBetweenCells + gridSize / 2,
            (placementLayer.GetLength(1) / 2 * gridSize) + (placementLayer.GetLength(1) - 1) * spaceingBetweenCells + gridSize / 2);

        for (int x = 0; x < placementLayer.GetLength(0); x++)
        {
            for (int y = 0; y < placementLayer.GetLength(1); y++)
            {
                //Placeholder level
                if(x == 0 && y == 0)
                    placementLayer[x, y] = false;
                else if(y == 2 && x == 3)
                    placementLayer[x, y] = false;
                else
                    placementLayer[x, y] = true;

                //make grid
                if(placementLayer[x, y])
                {
                    GameObject spawn = Instantiate(cellPrefab, transform);
                    spawn.name = $"Cell: {x}, {y}";

                    spawn.transform.localPosition = new Vector2(
                        x * gridSize + (x - 1) * spaceingBetweenCells - targetOffset.x / 2,
                        y * gridSize + (y - 1) * spaceingBetweenCells - targetOffset.y / 2);

                    spawn.GetComponent<Cell>().gridPos = new Vector2Int(x, y);
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
