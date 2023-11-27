using ES3Types;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{    
    [Header("Refrences")]
    [SerializeField] protected LevelManager levelManager;

    [Header("Grid system")]
    public Cell[,] grid;
    [SerializeField] protected GameObject cellPrefab;
    [SerializeField] protected float gridSize;
    [SerializeField] protected float spaceingBetweenCells = .2f;

    public Action onChange;

    [SerializeField] protected GameObject testDot;

    private bool testLoadedLevel = false;


    private void Update()
    {
        if (!testLoadedLevel && Input.GetKeyDown(KeyCode.K))
        {
            testLoadedLevel = true;
            levelManager.LoadLevel(0);
            Debug.Log("T");
        }
    }

    public void LoadLevel(LevelData level)
    {
        Vector2Int levelSize = new Vector2Int(level.levelCells.GetLength(0), level.levelCells.GetLength(1));
        grid = new Cell[levelSize.x, levelSize.y];

        Vector2 gridStartPosition = new Vector2(
            ((levelSize.x - 1) * (gridSize + spaceingBetweenCells) - spaceingBetweenCells) / 2,
            ((levelSize.y - 1) * (gridSize + spaceingBetweenCells) - spaceingBetweenCells) / 2);
        

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                //make grid
                if(!level.levelCells[x, y].isNullCell)
                {
                    //Make Cell
                    GameObject cellSpawn = Instantiate(cellPrefab, transform);
                    cellSpawn.name = $"Cell: {x}, {y}";

                    //Placement
                    Vector2 spawnPoint = new Vector2(
                        x * (gridSize + spaceingBetweenCells) - gridStartPosition.x - spaceingBetweenCells / 2,
                        y * (gridSize + spaceingBetweenCells) - gridStartPosition.y - spaceingBetweenCells / 2
                        );
                    cellSpawn.GetComponent<RectTransform>().anchoredPosition = spawnPoint;

                    //Setup cell
                    Cell cell = cellSpawn.GetComponent<Cell>();
                    cell.ConvertToCell(level.levelCells[x, y]);
                    grid[x, y] = cell;
                    cell.gridPos = new Vector2Int(x, y);

                    //Spawn level
                    if (cell.occupying != null)
                    {
                        //Make dot & get component
                        GameObject spawn = Instantiate(testDot, cellSpawn.transform);
                        Dot makedDot = spawn.GetComponent<Dot>();

                        //Load saved dot
                        SerializableDot seriDot = level.levelCells[x, y].occupying;
                        Dot savedDot = SaveConverter.ConvertToDot(seriDot);
                        makedDot.Setup(savedDot.dotType);

                        //Place on Board
                        PlaceDot(new Vector2Int(x, y), makedDot);
                    }
                }
            }
        }
    }

    #region Board interaction
    public bool PlaceDot(Vector2Int coordinat, Dot targetDot)
    {
        if (grid[coordinat.x, coordinat.y].occupying == null)
        {
            grid[coordinat.x, coordinat.y].occupying = targetDot;
            GameObject targetCell = grid[coordinat.x, coordinat.y].gameObject;
            targetDot.transform.position = targetCell.transform.position;
            targetDot.transform.parent = targetCell.transform;
            return true;
        }
        else
            return false;
    }

    public bool PlacePiece(Vector2Int coordinats, Piece piece)
    {
        bool canPlace = true;

        //Does place exits
        for (int i = 0; i < piece.dotsArray.Length; i++)
        {
            Vector2Int coor = new Vector2Int(coordinats.x + (int)piece.gridPosArray[i].x, coordinats.y + (int)piece.gridPosArray[i].y);
            if (coor.x >= grid.GetLength(0) || coor.y >= grid.GetLength(1)
                || coor.x < 0 || coor.y < 0
                || grid[coor.x, coor.y] == null)
            {
                return false;
            }
        }

        //Check if all places are free
        for (int i = 0; i < piece.dotsArray.Length; i++)
        {
            Debug.Log(GetDot(coordinats + new Vector2Int((int)piece.gridPosArray[i].x, (int)piece.gridPosArray[i].y)));

            if (GetDot(coordinats + new Vector2Int((int)piece.gridPosArray[i].x, (int)piece.gridPosArray[i].y)) != null)
            {
                canPlace = false; 
                break;
            }
        }

        //Place dots
        if(canPlace)
        {
            for (int i = 0; i < piece.dotsArray.Length; i++)
            {
                Debug.Log($"{i}: {coordinats + new Vector2Int((int)piece.gridPosArray[i].x, (int)piece.gridPosArray[i].y)}");
                PlaceDot(coordinats + new Vector2Int((int)piece.gridPosArray[i].x, (int)piece.gridPosArray[i].y), piece.dotsArray[i]);
            }
        }

        return canPlace;
    }

    public IOccupying GetDot(Vector2Int coordinat)
    {
        IOccupying result = null;
        if (grid[coordinat.x, coordinat.y].occupying != null && grid[coordinat.x, coordinat.y].occupying is Dot)
            result = grid[coordinat.x, coordinat.y].occupying;

        return result;
    }

    public List<Dot> GetDots(Vector2Int[] coordinats) 
    {
        List<Dot> result = new List<Dot> ();

        foreach (Vector2Int coor in coordinats)
        {
            if(grid[coor.x, coor.y].occupying != null)
                result.Add((Dot)grid[coor.x, coor.y].occupying);
        }

        return result;
    }
    
    public void RemoveDot(Vector2Int coodinat)
    {
        grid[coodinat.x, coodinat.y].occupying = null;
    }

    public void PickupPiece(Piece targetPiece)
    {
        foreach(Dot d in targetPiece.dotsArray)
        {
            Vector2Int coordinats = d.GetComponentInParent<Cell>().gridPos;
            RemoveDot(coordinats);
            d.transform.SetParent(targetPiece.transform);
        }
    }
    #endregion
}