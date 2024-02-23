using System;
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

    [SerializeField] protected GameObject placeGoalPrefab;

    public Action onChange;

    [SerializeField] protected GameObject dotPrefab;


    public void LoadLevel(LevelSO level)
    {
        //Remove load level
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        //Calculations
        Vector2Int levelSize = new Vector2Int((int)level.levelGrid.boardSize.x, (int)level.levelGrid.boardSize.y);
        grid = new Cell[levelSize.x, levelSize.y];

        Vector2 gridStartPosition = new Vector2(
            ((levelSize.x - 1) * (gridSize + spaceingBetweenCells) - spaceingBetweenCells) / 2,
            ((levelSize.y - 1) * (gridSize + spaceingBetweenCells) - spaceingBetweenCells) / 2);

        //Piece position
        List<Vector2> piecePositions = new List<Vector2>();
        for (int i = 0; i < level.levelPieces.Length; i++)
        {
            for (int k = 0; k < level.levelPieces[i].dotPositions.Length; k++)
                piecePositions.Add(level.levelPieces[i].dotPositions[k] + level.levelPieces[i].gridPosRef);
        }

        //Dots
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                //See if cell is active
                if (!level.levelGrid.activeCells[y * (int)level.levelGrid.boardSize.x + x])
                    continue;

                //Make Cell
                GameObject cellSpawn = Instantiate(cellPrefab, transform);
                cellSpawn.name = $"Cell: {x}, {y}";

                //Placement
                Vector2 spawnPoint = new Vector2(
                    x * (gridSize + spaceingBetweenCells) - gridStartPosition.x - spaceingBetweenCells / 2,
                    y * (-(gridSize + spaceingBetweenCells)) + gridStartPosition.y - spaceingBetweenCells / 2
                    );
                cellSpawn.GetComponent<RectTransform>().anchoredPosition = spawnPoint;

                //Setup cell
                Cell cell = cellSpawn.GetComponent<Cell>();
                grid[x, y] = cell;
                cell.gridPos = new Vector2Int(x, y);

                //Part of a piece
                if (piecePositions.Contains(new Vector2(x, y)))
                    continue;
                //Open space
                int targetDotIndex = y * (int)level.levelGrid.boardSize.x + x;
                if (level.levelGrid.dots[targetDotIndex] == DotType.Null)
                    continue;

                //Make dot & get component
                GameObject spawn = Instantiate(dotPrefab, cellSpawn.transform);
                Dot newDot = spawn.GetComponent<Dot>();

                //Load saved dot
                newDot.Setup(level.levelGrid.dots[targetDotIndex]);

                newDot.cell = cell;
                cell.occupying = newDot;

                //Place on Board
                PlaceDot(new Vector2Int(x, y), newDot);
            }
        }

        foreach (var item in level.levelPlaceGoals)
            PlaceGoal.MakeGoal(item, grid, placeGoalPrefab);
    }

    #region Board interaction
    public bool PlaceDot(Vector2Int coordinate, Dot targetDot)
    {
        if (grid[coordinate.x, coordinate.y].occupying == null)
        {
            grid[coordinate.x, coordinate.y].occupying = targetDot;
            GameObject targetCell = grid[coordinate.x, coordinate.y].gameObject;

            targetDot.transform.position = targetCell.transform.position;
            targetDot.cell = targetCell.GetComponent<Cell>();
            return true;
        }
        else
            return false;
    }

    public bool PlacePiece(Vector2Int inputCoordinates, Piece piece)
    {
        bool canPlace = true;

        //Calculate target positions
        List<Vector2Int> coordinatesResult = new List<Vector2Int>();

        for (int i = 0; i < piece.dotsArray.Length; i++)
        {
            //Don't know why y-axis need to be negetiv to work
            coordinatesResult.Add(new Vector2Int(
                inputCoordinates.x + piece.pieceCenter.x + (int)piece.gridPosArray[i].x, 
                inputCoordinates.y + piece.pieceCenter.y - (int)piece.gridPosArray[i].y)); 
        }

        //Does place exits
        for (int i = 0; i < coordinatesResult.Count; i++)
        {
            if (coordinatesResult[i].x >= grid.GetLength(0) || coordinatesResult[i].y >= grid.GetLength(1)
                || coordinatesResult[i].x < 0 || coordinatesResult[i].y < 0
                || grid[coordinatesResult[i].x, coordinatesResult[i].y] == null)
            {
                return false;
            }
        }

        //Check if all places are free
        for (int i = 0; i < piece.dotsArray.Length; i++)
        {
            if (GetDot(coordinatesResult[i]) != null)
            {
                canPlace = false;
                break;
            }
        }

        //Place dots
        if (canPlace)
        {
            //Set Piece scale
            piece.ChangeState(Piece.pieceStats.normal);

            for (int i = 0; i < piece.dotsArray.Length; i++)
            {
                PlaceDot(coordinatesResult[i], piece.dotsArray[i]);
                piece.dotsArray[i].cell = grid[coordinatesResult[i].x, coordinatesResult[i].y];
            }

        }

        return canPlace;
    }

    public IOccupying GetDot(Vector2Int coordinate)
    {
        IOccupying result = null;
        if (grid[coordinate.x, coordinate.y].occupying != null && grid[coordinate.x, coordinate.y].occupying is Dot)
            result = grid[coordinate.x, coordinate.y].occupying;

        return result;
    }

    public List<Dot> GetDots(Vector2Int[] coordinates)
    {
        List<Dot> result = new List<Dot>();

        foreach (Vector2Int coor in coordinates)
        {
            if (grid[coor.x, coor.y].occupying != null)
                result.Add((Dot)grid[coor.x, coor.y].occupying);
        }

        return result;
    }

    public void RemoveDot(Vector2Int coodinate)
    {
        if (grid[coodinate.x, coodinate.y].occupying is Dot dot)
        {
            dot.cell = null;
        }
        grid[coodinate.x, coodinate.y].occupying = null;
    }

    public void PickupPiece(Piece targetPiece)
    {
        targetPiece.EnforceDotPositions();
        foreach (Dot d in targetPiece.dotsArray)
        {
            Vector2Int coordinates = d.cell.gridPos;
            RemoveDot(coordinates);
        }
    }
    #endregion
}