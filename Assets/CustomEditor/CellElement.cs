using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CellElement : Image
{
    //public Cell cell { get; private set; }
    public Vector2Int gridCoordinates { get; private set; }
    private LevelEditor levelEditor;

    public bool turnedOff { get; private set; } = false;

    
    public bool partOfPiece { get; private set; } = false;
    public PieceElement piece { get; private set; }
    public List<ShapeGoalElement> partOfShapeGoals { get; private set; } = new List<ShapeGoalElement>();

    public DotElement holding;

    //Color control
    public CellColorState myColorState { get; private set; } = CellColorState.normal;
    Color[] cellColorState = new Color[]
        {
            new Color(.2f, 1, 0.333f),
            new Color(0, .7f, .7f),
            new Color(0.01f, .6f, .5f),
            new Color(1, 0, 0.666f),
            new Color(1, .2f, 1),
            Color.black,
            Color.white
        };

    public CellElement(Cell cell, Vector2Int coordinats, LevelEditor editor)
    {
        levelEditor = editor;
        //this.cell = cell;
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

    public void SetDefultColor()
    {
        if (myColorState == CellColorState.partGoalAndPiece)
        {
            if(!partOfPiece)
                myColorState = CellColorState.partGoal;
            if (partOfShapeGoals.Count == 0)
                myColorState = CellColorState.partPiece;
        }
        else if(partOfShapeGoals.Count > 1)
            myColorState |= CellColorState.partGoal;
        else
            myColorState = CellColorState.normal;

        this.tintColor = cellColorState[(int)myColorState];
    }
    public void ChangeCellColor(CellColorState targetColor)
    {
        myColorState = targetColor;
        this.tintColor = cellColorState[(int)targetColor];
    }

    public void SetDot(DotElement dot)
    {
        holding = dot;
        this.Add(dot);
    }
    public void ChangeDotColor()
    {
        if (holding != null && (!partOfPiece || partOfShapeGoals.Count == 0))
        {
            holding.ChangeColor();
        }
    }


    public void RemoveDot()
    {
        if (holding == null)
            return;

        //Remove Pieces
        if (partOfPiece)
            levelEditor.RemovePiece(piece);

        //Remove Shape goals
        levelEditor.RemoveGoal(partOfShapeGoals, this);
        partOfShapeGoals.Clear();

        //Remove dot
        this.Remove(holding);
        holding = null;

        //Change color
        myColorState = CellColorState.normal;
        this.tintColor = cellColorState[(int)myColorState];
    }

    public void SetPiece(PieceElement pieceElement)
    {
        partOfPiece = true;
        piece = pieceElement;

        //Change color
        if (myColorState == CellColorState.partGoal)
            myColorState = CellColorState.partGoalAndPiece;
        else
            myColorState = CellColorState.partPiece;
        this.tintColor = cellColorState[(int)myColorState];
    }

    public void RemovePiece()
    {
        partOfPiece = false;
        piece = null;

        SetDefultColor();
    }

    public void SetGoal(ShapeGoalElement shapeGoalElement)
    {
        partOfShapeGoals.Add(shapeGoalElement);

        if (myColorState == CellColorState.partPiece)
            myColorState = CellColorState.partGoalAndPiece;
        else
            myColorState = CellColorState.partGoal;

        this.tintColor = cellColorState[(int)myColorState];
    }

    public void RemoveGoal()
    {
        partOfShapeGoals.Clear();

        SetDefultColor();
    }

    public void RemoveGoal(ShapeGoalElement shapeGoalElement)
    {
        if (partOfShapeGoals.Contains(shapeGoalElement))
            partOfShapeGoals.Remove(shapeGoalElement);

        SetDefultColor();
    }


    private void OnMouseDown(MouseDownEvent evt)
    {
        // Call the OnCellClicked method of the LevelEditor window
        levelEditor.OnCellClicked(this, evt.button);
    }


    public void TurnOffCell() => SetActiveState(turnedOff);
    public void SetActiveState(bool targetState)
    {
        if (targetState)
        {
            turnedOff = false;
            SetDefultColor();
        }
        else
        {
            turnedOff = true;

            myColorState = CellColorState.turnedOff;
            this.tintColor = cellColorState[(int)myColorState];

            RemoveDot();
        }
    }

}
