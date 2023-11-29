using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CellElement : Image
{
    public Cell cell { get; private set; }
    private Vector2Int gridCoordinates;
    private LevelEditor levelEditor;

    public DotElement holding;

    public CellElement(Cell cell, Vector2Int coordinats, LevelEditor editor)
    {
        levelEditor = editor;
        this.cell = cell;
        style.width = 50;
        style.height = 50;

        cell.gridPos = coordinats;
        gridCoordinates = coordinats;

        // Create an Image to hold the sprite
        this.scaleMode = ScaleMode.ScaleToFit;
        this.sprite = Resources.Load<Sprite>("Square");

        // Add event handlers for mouse down and up events
        RegisterCallback<MouseDownEvent>(OnMouseDown);

        //Flexbox
        style.flexDirection = FlexDirection.Row;
        style.justifyContent = Justify.Center;
        style.alignItems = Align.Center;
    }

    public void SetDot(DotElement dot)
    {
        holding = dot;
        this.Add(dot);
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        // Call the OnCellClicked method of the LevelEditor window
        levelEditor.OnCellClicked(this);
    }

    public void ChangeShowSprite() => this.SetEnabled(!this.enabledSelf);
    public void ChangeShowSprite(bool targetState) => this.SetEnabled(targetState);

}
