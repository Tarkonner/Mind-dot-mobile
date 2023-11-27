using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditBoard : Board
{
    [SerializeField] GameObject cellSpawner;

    public GameObject[,] spawnerGrid;

    private void Start()
    {
        CellEditor();
    }

    public void CellEditor()
    {
        Vector2Int levelSize = new Vector2Int(7, 7); //7 is the max size of cells there can be on screen
        grid = new Cell[levelSize.x, levelSize.y];
        spawnerGrid = new GameObject[levelSize.x, levelSize.y];

        Vector2 gridStartPosition = new Vector2(
            ((levelSize.x - 1) * (gridSize + spaceingBetweenCells) - spaceingBetweenCells) / 2,
            ((levelSize.y - 1) * (gridSize + spaceingBetweenCells) - spaceingBetweenCells) / 2);

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                //Make Spawn cell
                GameObject cellSpawn = Instantiate(cellSpawner, transform);
                cellSpawn.name = $"Cell: {x}, {y}";
                spawnerGrid[x, y] = cellSpawn;

                cellSpawn.GetComponent<CellSpawner>().gridPosition = new Vector2Int(x, y);

                //Placement
                Vector2 spawnPoint = new Vector2(
                    x * (gridSize + spaceingBetweenCells) - gridStartPosition.x - spaceingBetweenCells / 2,
                    y * (gridSize + spaceingBetweenCells) - gridStartPosition.y - spaceingBetweenCells / 2
                    );
                cellSpawn.GetComponent<RectTransform>().anchoredPosition = spawnPoint;

                //Make Cell
                cellSpawn = Instantiate(cellPrefab, transform);
                cellSpawn.name = $"Cell: {x}, {y}";
                grid[x, y] = cellSpawn.GetComponent<Cell>();
                //Placement
                cellSpawn.GetComponent<RectTransform>().anchoredPosition = spawnPoint;
                cellSpawn.GetComponent<Cell>().gridPos = new Vector2Int(x, y);

                cellSpawn.SetActive(false);
            }
        }
    }

    [ContextMenu("Construct")]
    public Cell[,] GetConstructetGrid()
    {
        int minX = grid.GetLength(0) - 1, minY = grid.GetLength(1) - 1, maxX = 0, maxY = 0;
        for (int x = 0; x < grid.GetLength(1); x++)
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                if (grid[x, y].gameObject.activeSelf)
                {
                    if(x < minX) minX = x;
                    if(y < minY) minY = y;
                    if(y > maxX) maxX = y;
                    if(x > maxY) maxY = x;
                }
            }
        }

        Cell[,] result = new Cell[maxX - minX + 1, maxY - minY + 1];

        Debug.Log($"Maked grid: [x: {result.GetLength(0)} y: {result.GetLength(1)}]");

        return result;
    }
}
