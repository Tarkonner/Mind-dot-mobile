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


    #region Color
    private int colorGoalCount = 0;
    private bool colorPartPiece = false;
    public CellColorState myColorState { get; private set; } = CellColorState.normal;

    private Dictionary<CellColorState, Color> cellColorState = new Dictionary<CellColorState, Color>
    {
        { CellColorState.normal, Color.white },
        { CellColorState.turnedOff, new Color(.2f, .2f, .2f) },
        { CellColorState.choosenPiece, Color.cyan },
        { CellColorState.choosenGoal, new Color(1f, 0.85f, 0.43f) },
        { CellColorState.partGoal, new Color(.2f, .9f, .4f) },
        { CellColorState.partPiece, new Color(0.01f, .6f, .5f) },
        { CellColorState.partGoalAndPiece, new Color(1, 0, 0.666f) },
    };

    public void SetDefaultColor()
    {
        ChangeCellColor(CellColorState.normal);
        colorGoalCount = 0;
        colorPartPiece = false;
    }

    public void UnsubColor(CellColorState color = CellColorState.normal)
    {
        if (color == CellColorState.partGoal)
            colorGoalCount--;
        if (color == CellColorState.partPiece)
            colorPartPiece = false;

        if (colorGoalCount > 0 && colorPartPiece)
            ChangeCellColor(CellColorState.partGoalAndPiece);
        else if (colorGoalCount > 0 && !colorPartPiece && color == CellColorState.partPiece)
            ChangeCellColor(CellColorState.partGoal);
        else if(colorGoalCount > 0)
        {
            //To stop it to to in else
        }
        else if (colorPartPiece)
            ChangeCellColor(CellColorState.partPiece);
        else
            ChangeCellColor(CellColorState.normal);
    }

    public void ChangeCellColor(CellColorState targetColor)
    {
        //Save color
        if (targetColor == CellColorState.partGoal)
            colorGoalCount++;
        if (targetColor == CellColorState.partPiece)
            colorPartPiece = true;
        

        //Part of goal & piece
        if(colorPartPiece && colorGoalCount > 0)
            myColorState = CellColorState.partGoalAndPiece;
        else
            myColorState = targetColor;

        //Set color
        this.tintColor = cellColorState[myColorState];
    }

    #endregion

    public CellElement(Vector2Int coordinats, LevelEditor editor)
    {
        levelEditor = editor;
        style.width = 50;
        style.height = 50;

        cellData.gridCoordinates = coordinats;

        // Create an Image to hold the sprite
        this.scaleMode = ScaleMode.ScaleToFit;
        this.sprite = Resources.Load<Sprite>("GridElementSprite");

        // Add event handlers for mouse down and up events
        RegisterCallback<MouseDownEvent>(OnMouseDown);

        //Flexbox
        style.flexDirection = FlexDirection.Row;
        style.justifyContent = Justify.Center;
        style.alignItems = Align.Center;
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
        if (piece != null)
            levelEditor.RemovePiecesDots(piece);

        //Remove Shape goals
        levelEditor.RemoveGoal(partOfShapeGoals, this);
        partOfShapeGoals.Clear();

        //Remove dot
        this.Remove(dotRef);
        dotRef = null;
        cellData.holding = null;

        //Change color
        SetDefaultColor();
    }

    public void SetPiece(PieceElement pieceElement)
    {
        cellData.partOfPiece = true;
        piece = pieceElement;

        //Change color
        ChangeCellColor(CellColorState.partPiece);
    }

    public void RemovePiece()
    {
        cellData.partOfPiece = false;
        piece = null;

        UnsubColor(CellColorState.partPiece);
    }

    public void SetGoal(ShapeGoalElement shapeGoalElement)
    {
        partOfShapeGoals.Add(shapeGoalElement);

        ChangeCellColor(CellColorState.partGoal);
    }

    public void RemoveGoal()
    {
        partOfShapeGoals.Clear();
        UnsubColor(CellColorState.partGoal);
    }

    public void RemoveGoal(ShapeGoalElement shapeGoalElement)
    {
        if (partOfShapeGoals.Contains(shapeGoalElement))
            partOfShapeGoals.Remove(shapeGoalElement);

        UnsubColor(CellColorState.partGoal);
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
            SetDefaultColor();
        }
        else
        {
            cellData.turnedOff = true;

            ChangeCellColor(CellColorState.turnedOff);

            RemoveDot();
        }
    }

    public void AddPlacementGoal(DotType type)
    {
        //RemovePlacementGoal();
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
