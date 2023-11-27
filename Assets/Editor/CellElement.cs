using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CellElement : VisualElement
{
    public Cell cell { get; private set; }
    private Image image;
    private Vector2Int gridCoordinates;
    private LevelEditor levelEditor;

    public CellElement(Cell cell, Vector2Int coordinats, LevelEditor editor)
    {
        levelEditor = editor;
        this.cell = cell;
        style.width = 50;
        style.height = 50;

        cell.gridPos = coordinats;
        gridCoordinates = coordinats;

        // Create an Image to hold the sprite
        image = new Image();
        image.scaleMode = ScaleMode.ScaleToFit;
        Add(image);

        // Add event handlers for mouse down and up events
        RegisterCallback<MouseDownEvent>(OnMouseDown);
    }

    public void SetSprite(Sprite sprite)
    {
        // Set the sprite for the cell
        image.sprite = sprite;
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        // Call the OnCellClicked method of the LevelEditor window
        levelEditor.OnCellClicked(this);
    }

    public void ChangeShowSprite() => image.SetEnabled(!image.enabledSelf);
}
