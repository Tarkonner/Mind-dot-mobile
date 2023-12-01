using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CellElement : Image
{
    public Cell cell { get; private set; }
    public Vector2Int gridCoordinates { get; private set; }
    private LevelEditor levelEditor;

    public bool turnedOff { get; private set; } = false;

    
    public bool partOfPiece { get; private set; } = false;
    public PieceElement piece { get; private set; }
    public bool partOfGoal { get; private set; } = false;
    public ShapeGoalElement shapeGoal { get; private set; }


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

    public void SetDefultColor() => this.tintColor = Color.white;

    public void SetDot(DotElement dot)
    {
        holding = dot;
        this.Add(dot);
    }
    public void RemoveDot()
    {
        if (holding == null)
            return;

        this.Remove(holding);
        holding = null; 
    }

    public void SetPiece(PieceElement pieceElement)
    {
        partOfPiece = true;
        piece = pieceElement;

        this.tintColor = Color.grey;
    }

    public void RemovePiece()
    {
        partOfPiece = false;
        piece = null;

        SetDefultColor();
    }

    public void SetGoal(ShapeGoalElement shapeGoalElement)
    {
        partOfGoal = true;
        shapeGoal = shapeGoalElement;

        this.tintColor = Color.green;
    }

    public void RemoveGoal()
    {
        partOfGoal = false; 
        shapeGoal = null;

        SetDefultColor();
    }


    private void OnMouseDown(MouseDownEvent evt)
    {
        // Call the OnCellClicked method of the LevelEditor window
        levelEditor.OnCellClicked(this);
    }


    public void ChangeShowSprite() => ChangeShowSprite(turnedOff);
    public void ChangeShowSprite(bool targetState)
    {
        if (targetState)
        {
            turnedOff = false;
            SetDefultColor();
        }
        else
        {
            turnedOff = true;
            this.tintColor = Color.black;

            RemoveDot();
        }
    }

}
