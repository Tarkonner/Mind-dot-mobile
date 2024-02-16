using SharedData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CellElement : Image
{
    private LevelEditor levelEditor;
    public DotElement dotRef { get; private set; }

    public CellData cellData { get; private set;} = new CellData();
    public PieceElement piece { get; private set; }
    public List<ShapeGoalElement> partOfShapeGoals { get; private set; } = new List<ShapeGoalElement>();

    public PlaceGoalElement placeGoal { get; private set; }

    //Color control
    public CellColorState myColorState { get; private set; } = CellColorState.normal;
    Color[] cellColorState = new Color[]
        {
            new Color(.2f, 1, 0.333f),
            new Color(0, .7f, .7f),
            new Color(0.01f, .6f, .5f),
            new Color(1, 0, 0.666f),
            new Color(1, .2f, 1),
            new Color(.2f, .2f, .2f),
            Color.white
        };

    public CellElement(Vector2Int coordinats, LevelEditor editor)
    {
        levelEditor = editor;
        style.width = 50;
        style.height = 50;

        cellData.gridCoordinates = coordinats;

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
            if(!cellData.partOfPiece)
                myColorState = CellColorState.partGoal;
            if (partOfShapeGoals.Count == 0)
                myColorState = CellColorState.partPiece;
        }
        else if(partOfShapeGoals.Count > 1)
            myColorState |= CellColorState.partGoal;
        else if(cellData.turnedOff)
            myColorState = CellColorState.turnedOff;
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
        dotRef = dot;
        cellData.holding = dot.DotData;
        this.Add(dot);
    }
    public void ChangeDotColor()
    {
        if (dotRef != null && (!cellData.partOfPiece || partOfShapeGoals.Count == 0))
            dotRef.ChangeColor();
    }


    public void RemoveDot()
    {
        if (cellData.holding == null)
            return;

        //Remove Pieces
        if (cellData.partOfPiece)
            levelEditor.RemovePiece(piece);

        //Remove Shape goals
        levelEditor.RemoveGoal(partOfShapeGoals, this);
        partOfShapeGoals.Clear();

        //Remove dot
        dotRef = null;
        cellData.holding = null;

        //Change color
        myColorState = CellColorState.normal;
        this.tintColor = cellColorState[(int)myColorState];
    }

    public void SetPiece(PieceElement pieceElement)
    {
        cellData.partOfPiece = true;
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
        cellData.partOfPiece = false;
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


    public void TurnOffCell() => SetActiveState(cellData.turnedOff);
    public void SetActiveState(bool targetState)
    {
        if (targetState)
        {
            cellData.turnedOff = false;
            SetDefultColor();
        }
        else
        {
            cellData.turnedOff = true;

            myColorState = CellColorState.turnedOff;
            this.tintColor = cellColorState[(int)myColorState];

            RemoveDot();
            SetDefultColor();
        }
    }

    public void AddPlacementGoal(DotType type)
    {
        RemovePlacementGoal();
        placeGoal = new PlaceGoalElement(type, this);
        Add(placeGoal);
    }
    public void RemovePlacementGoal()
    {
        if (placeGoal != null)
        {
            this.Remove(placeGoal);
            placeGoal = null;
        }
    }
}
