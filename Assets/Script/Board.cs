using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Board : ScaleAnimations
{
    public static Board Instance;

    [Header("Refrences")]
    [SerializeField] protected LevelManager levelManager;

    [Header("Grid system")]
    public Cell[,] grid;
    [SerializeField] protected GameObject cellPrefab;
    [SerializeField] protected float gridSize;
    [SerializeField] protected float spaceingBetweenCells = .2f;

    public Action onChange;

    [Header("Animation")]
    [SerializeField] private float cellAnimationTime = .5f;
    [SerializeField] private float dotAnimationTime = .5f;
    [SerializeField] private float animateOutTime = .5f;
    private List<GameObject> allDots = new List<GameObject>();
    private List<GameObject> allCells = new List<GameObject>();


    private void Awake()
    {
        Instance = this;
    }

    public void LoadLevel(LevelSO level)
    {
        if (transform.childCount > 0)
        {
            ScaleOutLiniar(allDots, animateOutTime);
            StartCoroutine(MakeBoard(animateOutTime, level));
        }
        else
            StartCoroutine(MakeBoard(0, level));
    }

    IEnumerator MakeBoard(float waitTime, LevelSO level)
    {
        yield return new WaitForSeconds(waitTime + .05f);

        //Remove load level
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            //Here is both Cells and Pieces there is placed on board

            //Dot to pool
            GameObject target = transform.GetChild(i).gameObject;
            Dot foundDot = target.GetComponentInChildren<Dot>();
            if(foundDot != null)
                DotPool.instance.Release(foundDot);

            //Cell to pool
            if(target.TryGetComponent(out Cell foundCell))
                CellPool.instance.Release(foundCell);

            allDots.Clear();
            allCells.Clear();
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
                Cell cellSpawn = CellPool.instance.Get();
                cellSpawn.transform.parent = transform;
                cellSpawn.name = $"Cell: {x}, {y}";

                //Animate cell
                allCells.Add(cellSpawn.gameObject);

                //Placement
                Vector2 spawnPoint = new Vector2(
                    x * (gridSize + spaceingBetweenCells) - gridStartPosition.x - spaceingBetweenCells / 2,
                    y * (-(gridSize + spaceingBetweenCells)) + gridStartPosition.y - spaceingBetweenCells / 2
                    );
                cellSpawn.GetComponent<RectTransform>().anchoredPosition = spawnPoint;

                //Setup cell
                grid[x, y] = cellSpawn;
                cellSpawn.gridPos = new Vector2Int(x, y);

                //Part of a piece
                if (piecePositions.Contains(new Vector2(x, y)))
                    continue;
                //Open space
                int targetDotIndex = y * (int)level.levelGrid.boardSize.x + x;
                if (level.levelGrid.dots[targetDotIndex] == DotType.Null)
                    continue;

                //Make dot with save data
                GameObject spawn = DotPool.instance.GetDot(level.levelGrid.dots[targetDotIndex]);
                spawn.transform.parent = cellSpawn.transform;

                //Place on Board
                PlaceDot(new Vector2Int(x, y), spawn.GetComponent<Dot>());

                //Ready to animate
                spawn.transform.localScale = Vector2.zero;
                allDots.Add(spawn);
            }
        }

        ScaleInLiniar(allCells, cellAnimationTime);

        StartCoroutine(DotAnimation());
    }

    #region Animations
    IEnumerator DotAnimation()
    {
        yield return new WaitForSeconds(cellAnimationTime);
        ScaleInLiniar(allDots, dotAnimationTime);
    }
    #endregion

    #region Board interaction
    public bool PlaceDot(Vector2Int coordinate, Dot targetDot)
    {
        if (grid[coordinate.x, coordinate.y].occupying == null)
        {
            grid[coordinate.x, coordinate.y].occupying = targetDot;
            GameObject targetCell = grid[coordinate.x, coordinate.y].gameObject;
                                  
            targetDot.gameObject.transform.position = targetCell.transform.position;
            //Cell
            Cell cell = targetCell.GetComponent<Cell>(); 
            targetDot.cell = cell;
            cell.occupying = targetDot;

            //Animation
            allDots.Add(targetDot.gameObject);

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
        Vector2Int posCalculation = inputCoordinates;
        for (int i = 0; i < piece.dotsArray.Length; i++)
        {
            if(i != 0)
            {
                Vector2 dif = piece.gridPosArray[i] - piece.gridPosArray[i - 1];
                posCalculation = posCalculation + new Vector2Int((int)dif.x, (int)dif.y);
            }
            coordinatesResult.Add(posCalculation); 
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