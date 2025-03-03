using System.Collections;
using System.Collections.Generic;
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
}
