using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractiveGrid 
{
    LevelEditor levelEditor;
    VisualElement root;
    public List<CellElement> cells { get; private set; } = new List<CellElement>();

    public const int gridSize = 6;

    public InteractiveGrid(VisualElement root, LevelEditor levelEditor)
    {
        this.root = root;
        this.levelEditor = levelEditor;

        MakeGrid();
    }

    void MakeGrid()
    {
        root.style.flexDirection = FlexDirection.Column;


        // Add cells to the grid
        // Create a new row element and add it to the grid
        VisualElement row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row; // Set the row to align horizontally
        root.Add(row);
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                var cellElement = new CellElement(new Vector2Int(j, i), levelEditor);

                // Add the cell to the current row
                row.Add(cellElement);

                // If the row is full, add it to the grid and start a new row
                if ((j + 1) % gridSize == 0)
                {
                    root.Add(row); // Add the full row to the grid
                    row = new VisualElement(); // Create a new row
                    row.style.flexDirection = FlexDirection.Row; // Set the row to align horizontally
                }

                cells.Add(cellElement);
            }
        }
    }

    public void ResizeGrid(Vector2 targetSize)
    {
        //Clamp
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                CellElement target = cells[y * gridSize + x];

                if (targetSize.x <= x || targetSize.y <= y)
                {
                    target.RemoveDot();
                    target.SetActiveState(false);
                }
                else
                    target.SetActiveState(true);
            }
        }
    }

    public Vector2 GridSize()
    {
        Vector2 boardSize = new Vector2(cells[0].cellData.gridCoordinates.x, cells[0].cellData.gridCoordinates.y);
        foreach (CellElement cell in cells)
        {
            if (cell.cellData.turnedOff) continue;
            if (cell.cellData.gridCoordinates.x > boardSize.x)
                boardSize.x = cell.cellData.gridCoordinates.x;
            if (cell.cellData.gridCoordinates.y > boardSize.y)
                boardSize.y = cell.cellData.gridCoordinates.y;
        }
        boardSize.x += 1;
        boardSize.y += 1;

        return boardSize;
    }

    public void MakeRandomCells()
    {
        // Turn all cells off
        foreach (CellElement item in cells)
            item.TurnOffCell();

        float outerChanceForDisable = .75f;
        float interChanceForDisable = .5f;

        // Level size
        Vector2Int levelSize = new Vector2Int((int)UnityEngine.Random.Range(3, gridSize + 1), (int)UnityEngine.Random.Range(3, gridSize + 1));

        List<List<CellElement>> layers = GetGridLayers(levelSize);

        for (int i = 0; i < layers.Count; i++)
        {
            float chance = outerChanceForDisable - ((outerChanceForDisable - interChanceForDisable) / (layers.Count - 1)) * i;

            foreach (CellElement item in layers[i])
            {
                float disableResult = UnityEngine.Random.Range(0f, 1f);
                if (disableResult < chance)
                    item.SetActiveState(true);
            }
        }
    }

    List<List<CellElement>> GetGridLayers(Vector2Int levelSize)
    {

        List<List<Vector2Int>> cellPos = GridLayersPositionValues(levelSize);

        List<List<CellElement>> layersResult = new List<List<CellElement>>();
        for (int i = 0; i < cellPos.Count; i++)
            layersResult.Add(new List<CellElement>());

        foreach (CellElement c in cells)
        {
            Vector2Int pos = c.cellData.gridCoordinates;

            for (int i = 0; i < cellPos.Count; i++)
            {
                for (int j = 0; j < cellPos[i].Count; j++)
                {
                    if(cellPos[i][j] == pos)
                    {
                        layersResult[i].Add(c);
                        break;
                    }
                }
            }
        }

        return layersResult;
    }

    List<List<Vector2Int>> GridLayersPositionValues(Vector2Int gridSize)
    {
        List<List<Vector2Int>> results = new List<List<Vector2Int>>();

        Vector2Int xValues = new Vector2Int(0, gridSize.x - 1);
        Vector2Int yValues = new Vector2Int(0, gridSize.y - 1);

        int layers = (Math.Min(gridSize.x, gridSize.y) + 1) / 2;
        List<Vector2Int> usetPositon = new List<Vector2Int>();

        for (int l = 0; l < layers; l++)
        {
            List<Vector2Int> values = new List<Vector2Int>();

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector2Int coordinat = new Vector2Int(x, y);

                    if (usetPositon.Contains(coordinat))
                        continue;

                    if (x == xValues.x || (x == xValues.y && xValues.y > xValues.x) ||
                        y == yValues.x || (y == yValues.y && yValues.y > yValues.x))
                    {
                        usetPositon.Add(coordinat);
                        values.Add(coordinat);
                    }
                }
            }

            results.Add(values);

            xValues = new Vector2Int(xValues.x + 1, xValues.y - 1);
            yValues = new Vector2Int(yValues.x + 1, yValues.y - 1);
        }

        return results;
    }
}
