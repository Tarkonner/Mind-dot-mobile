using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelEditor : EditorWindow
{
    //Editor
    VisualElement rightPanel;
    VisualElement grid;
    List<CellElement> cells = new List<CellElement>();

    int editTypeIndex = 0;
    int dotIndex = 0;

    //Resize grid
    IntegerField horizontal;
    IntegerField vertical;

    //Pieces
    List<CellElement> piecesSavedCells = new List<CellElement>();
    VisualElement pieceHolder;
    List<PieceElement> pieces = new List<PieceElement>();
    //Goals
    List<CellElement> goalSavedCells = new List<CellElement>();
    VisualElement goalHolder;
    List<ShapeGoalElement> shapeGoals = new List<ShapeGoalElement>();

    #region Buttons
    //Buttons
    private Button choosenButton;
    //Dot
    Button redDotButton;
    Button blueDotButton;
    Button yellowDotButton;
    //Piece
    Button choosePieceButton;
    Button changeRotationStateButton;
    Button removePieceButton;
    //Goal
    Button chooseGoalButton;
    Button removeGoalButton;

    #endregion

    [MenuItem("Tools/Level Editor")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<LevelEditor>();
        wnd.titleContent = new GUIContent("Level Editor");
    }


    public void CreateGUI()
    {
        //Create buttons
        //Dots
        redDotButton = new Button(() => { editTypeIndex = 1; dotIndex = 0; ChangeButtonColor(redDotButton); }) { text = "Red Dot" };
        blueDotButton = new Button(() => { editTypeIndex = 1; dotIndex = 1; ChangeButtonColor(blueDotButton); }) { text = "Blue Dot" };
        yellowDotButton = new Button(() => { editTypeIndex = 1; dotIndex = 1; ChangeButtonColor(yellowDotButton); }) { text = "Blue Dot" };
        //Pieces
        choosePieceButton = new Button(() => { editTypeIndex = 5; ChangeButtonColor(choosePieceButton); }) { text = "Mark piece dots" };
        changeRotationStateButton = new Button(() => { editTypeIndex = 9; ChangeButtonColor(changeRotationStateButton); }) { text = "Change rotation setting" };
        removePieceButton = new Button(() => { editTypeIndex = 7; ChangeButtonColor(removePieceButton); }) { text = "Remove piece" };
        //Goal
        chooseGoalButton = new Button(() => { editTypeIndex = 6; ChangeButtonColor(chooseGoalButton); }) { text = "Mark shape goal dots" };
        removeGoalButton = new Button(() => { editTypeIndex = 8; ChangeButtonColor(removeGoalButton); }) { text = "Remove Shape Goal" };


        // Create a two-pane view with the left pane being fixed with
        var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        // Add the panel to the visual tree by adding it as a child to the root element
        rootVisualElement.Add(splitView);
        // A TwoPaneSplitView always needs exactly two child elements
        var leftPanel = new ListViewContainer();
        rightPanel = new ListViewContainer();
        splitView.Add(leftPanel);
        splitView.Add(rightPanel);

        //Left planel
        //Grid
        leftPanel.Add(new Label("Grid size"));
        leftPanel.Add(new Button(() => { editTypeIndex = 0; }) { text = "Turn cells on & off" });
        horizontal = new IntegerField("Horizontal", 7);
        leftPanel.Add(horizontal);
        vertical = new IntegerField("Vertical", 7);
        leftPanel.Add(vertical);
        leftPanel.Add(new Button(() => { ResizeGrid(); }) { text = "Resize grid" });
        //Dots
        leftPanel.Add(new Label("Dots"));
        leftPanel.Add(redDotButton);
        leftPanel.Add(blueDotButton);
        leftPanel.Add(yellowDotButton);
        //Pieces
        leftPanel.Add(new Label("Pieces"));
        leftPanel.Add(choosePieceButton);
        leftPanel.Add(new Button(() => { MakePiece(); }) { text = "Make piece" });
        leftPanel.Add(changeRotationStateButton) ;
        leftPanel.Add(removePieceButton);
        //Goals
        leftPanel.Add(new Label("Goals"));
        leftPanel.Add(chooseGoalButton);
        leftPanel.Add(new Button(() => { MakeShapeGoal(); }) { text = "Make Shape Goal" });
        leftPanel.Add(removeGoalButton);
        //Board
        leftPanel.Add(new Label("Board"));
        leftPanel.Add(new Button(() => { ClearAll(); }) { text = "Reset board" });
        

        //Right panel
        // Create a grid layout
        grid = new VisualElement();
        grid.style.flexDirection = FlexDirection.Row;
        grid.style.flexWrap = Wrap.Wrap;
        grid.style.justifyContent = Justify.SpaceAround; // Optional: Add space around the items

        CellElement blueprint = new CellElement(new Cell(), Vector2Int.zero, this);
        int gridSize = 7;
        int spaceing = 2;


        // Set the width and height of the grid
        grid.style.width = gridSize * blueprint.style.width.value.value + gridSize * spaceing; // cellWidth is the width of each cell
        grid.style.height = gridSize * blueprint.style.height.value.value + gridSize * spaceing; // cellHeight is the height of each cell

        // Add cells to the grid
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                var cell = new Cell();
                cell.ConvertToCell(new SerializableCell { gridPos = new Vector2Int(i, j) }); //try change i & j placing
                var cellElement = new CellElement(cell, new Vector2Int(j, i), this);

                //Set spacing
                cellElement.style.marginRight = new StyleLength(spaceing / 2);
                cellElement.style.marginTop = new StyleLength(spaceing);

                grid.Add(cellElement);

                cells.Add(cellElement);
            }
        }

        // Add the grid to the right panel
        rightPanel.Add(grid);

        //Pieces
        rightPanel.Add(new Label("Pieces"));
        pieceHolder = new VisualElement();
        rightPanel.Add(pieceHolder);
        //Flexbox
        pieceHolder.style.flexDirection = FlexDirection.Row;
        pieceHolder.style.flexWrap = Wrap.Wrap;
        pieceHolder.style.justifyContent = Justify.SpaceAround;

        //Goals
        rightPanel.Add(new Label("Goals"));
        goalHolder = new VisualElement();
        rightPanel.Add(goalHolder);
        //Flexbox
        goalHolder.style.flexDirection = FlexDirection.Row;
        goalHolder.style.flexWrap = Wrap.Wrap;
        goalHolder.style.justifyContent = Justify.SpaceAround;
    }

    private void ResizeGrid()
    {
        if (horizontal.value > 7)
            horizontal.value = 7;
        if (vertical.value > 7)
            vertical.value = 7;

        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                CellElement target = cells[y * 7 + x];

                if (horizontal.value <= x || vertical.value <= y)
                {
                    target.SetActiveState(false);

                    target.RemoveDot();
                }
                else
                    target.SetActiveState(true);
            }
        }

        //Reset
        horizontal.value = 0;
        vertical.value = 0;
    }

    #region Switch machine
    public void OnCellClicked(CellElement cellElement, int buttonIndex)
    {
        switch (editTypeIndex)
        {
            //Cells
            case 0:
                cellElement.TurnOffCell();
                break;

            //Dots
            case 1:
                PlaceDot(cellElement, buttonIndex);
                break;
                
            //Remove
            case 4:
                cellElement.RemoveDot();
                break;

            //Pieces
            case 5:
                if (cellElement.turnedOff || cellElement.partOfPiece)
                    break;

                if (!piecesSavedCells.Contains(cellElement))
                {
                    piecesSavedCells.Add(cellElement);
                    cellElement.ChangeCellColor(CellColorState.choosenPiece);
                }
                else
                {
                    piecesSavedCells.Remove(cellElement);
                    cellElement.SetDefultColor();
                }
                break;

            //Goals
            case 6:
                if (cellElement.turnedOff)
                    break;

                if (!goalSavedCells.Contains(cellElement))
                {
                    goalSavedCells.Add(cellElement);
                    cellElement.ChangeCellColor(CellColorState.choosenGoal);
                }
                else
                {
                    goalSavedCells.Remove(cellElement);
                    cellElement.SetDefultColor();
                }
                break;
        }
    }
    

    public void ContentManagement(GridElement targetGrid)
    {
        if (editTypeIndex <= 6)
            return;

        switch (editTypeIndex)
        {
            //Remove piece
            case 7:
                if (targetGrid is PieceElement)
                    RemovePiece((PieceElement)targetGrid);
                break;


            //Remove goal
            case 8:
                if(targetGrid is ShapeGoalElement)
                    RemoveGoal((ShapeGoalElement)targetGrid);
                break;

            //No rotation piece
            case 9:
                if (targetGrid is PieceElement)
                {
                    PieceElement t = targetGrid as PieceElement;
                    t.ChangeRotationStatus();
                }
                break;
        }
    }
    #endregion

    private void PlaceDot(CellElement targetCell, int buttonIndex)
    {
        if (targetCell.turnedOff || targetCell.partOfPiece || targetCell.partOfShapeGoals.Count > 0)
            return;
             

        if (buttonIndex == 0) //Left click
        {
            if (targetCell.childCount > 0)
                targetCell.ChangeDotColor();
            else
            {
                DotElement targetDot = null;
                switch (dotIndex)
                {
                    case 0:
                        targetDot = new DotElement(DotType.Red);
                        break;
                    case 1:
                        targetDot = new DotElement(DotType.Blue);
                        break;
                    case 2:
                        targetDot = new DotElement(DotType.Yellow);
                        break;
                }

                targetCell.SetDot(targetDot);
            }
        }
        else if (buttonIndex == 1) //Right click
            targetCell.RemoveDot();
    }

    private void MakePiece()
    {
        GridElement grid = MakeGridElement(piecesSavedCells, new PieceElement(this));

        if (grid != null)
        {
            
            pieceHolder.Add(grid);
            piecesSavedCells.Clear();
        }
    }
    private void MakeShapeGoal()
    {
        GridElement grid = MakeGridElement(goalSavedCells, new ShapeGoalElement(this));

        if (grid != null)
        {
            goalHolder.Add(grid);
            goalSavedCells.Clear();
        }
    }

    private GridElement MakeGridElement(List<CellElement> targetElements, GridElement gridType)
    {
        if (targetElements.Count > 0)
        {
            //Change back color
            for (int i = 0; i < targetElements.Count; i++)
                targetElements[i].SetDefultColor();


            //Remove cells without dots
            for (int i = targetElements.Count - 1; i >= 0; i--)
            {
                if (targetElements[i].holding == null)
                    targetElements.RemoveAt(i);
            }

            if (targetElements.Count == 0)
                return null;            

            //Goal or piece
            GridElement spawnedGrid;
            if (gridType is PieceElement)
            {
                spawnedGrid = new PieceElement(this);
                pieces.Add((PieceElement)spawnedGrid);
            }
            else if (gridType is ShapeGoalElement)
            {
                spawnedGrid = new ShapeGoalElement(this);
                shapeGoals.Add((ShapeGoalElement)spawnedGrid);
            }
            else
            {
                Debug.LogError($"Not implementet gridtype: {gridType}");
                return null;
            }

            //Save siblings
            for (int i = 0; i < targetElements.Count; i++)
            {
                spawnedGrid.siblings.Add(targetElements[i]);
            }

            //Calculate position
            Vector2Int lowPoint = new Vector2Int(10, 10);
            Vector2Int highPoint = new Vector2Int(0, 0);
            for (int i = 0; i < targetElements.Count; i++)
            {
                //Low
                if (lowPoint.x > targetElements[i].gridCoordinates.x)
                    lowPoint.x = targetElements[i].gridCoordinates.x;
                if (lowPoint.y > targetElements[i].gridCoordinates.y)
                    lowPoint.y = targetElements[i].gridCoordinates.y;

                //High
                if (highPoint.x < targetElements[i].gridCoordinates.x)
                    highPoint.x = targetElements[i].gridCoordinates.x;
                if (highPoint.y < targetElements[i].gridCoordinates.y)
                    highPoint.y = targetElements[i].gridCoordinates.y;
            }

            for (int i = 0; i < targetElements.Count; i++)
            {
                //Set refence point
                Vector2Int targetCoor = targetElements[i].gridCoordinates - new Vector2Int(lowPoint.x, lowPoint.y);

                spawnedGrid.AddDot(targetCoor, targetElements[i].holding);

                //Change background color
                if (gridType is PieceElement)
                    targetElements[i].SetPiece((PieceElement)spawnedGrid);
                else if (gridType is ShapeGoalElement)
                    targetElements[i].SetGoal((ShapeGoalElement)spawnedGrid);
            }

            spawnedGrid.Construct();
            spawnedGrid.style.marginRight = new StyleLength(10); // Add right margin
            spawnedGrid.style.marginBottom = new StyleLength(10); // Add bottom margin

            //Set color
            for (int i = 0; i < targetElements.Count; i++)
            {
                if (targetElements[i].partOfPiece && gridType is ShapeGoalElement
                    || targetElements[i].partOfShapeGoals.Count > 0 && gridType is PieceElement)
                {
                    targetElements[i].ChangeCellColor(CellColorState.partGoalAndPiece);
                }
                else if (gridType is ShapeGoalElement)
                    targetElements[i].ChangeCellColor(CellColorState.partGoal);
                else if (gridType is PieceElement)
                    targetElements[i].ChangeCellColor(CellColorState.partPiece);
            }

            return spawnedGrid;
        }
        else
        {
            Debug.Log("No cells chosen");
            return null;
        }
    }

    #region Removel
    public void RemovePiece(PieceElement target)
    {
        for (int i = target.siblings.Count - 1; i >= 0; i--)
        {
            target.siblings[i].RemovePiece();
            target.siblings.RemoveAt(i);
        }

        pieces.Remove(target);
        pieceHolder.Remove(target);
    }
    public void RemoveGoal(ShapeGoalElement target)
    {
        for (int i = target.siblings.Count - 1; i >= 0; i--)
        {
            target.siblings[i].RemoveGoal();
        }

        shapeGoals.Remove(target);
        goalHolder.Remove(target);
    }
    public void RemoveGoal(List<ShapeGoalElement> target, CellElement targetCell)
    {
        HashSet<CellElement> allSiblings = new HashSet<CellElement>();
        for (int i = 0; i < target.Count; i++)
        {
            for (int j = 0; j < target[i].siblings.Count; j++)
            {
                CellElement cell = target[i].siblings[j];
                if(cell != targetCell)
                    allSiblings.Add(cell);
            }
        }

        //Remove from holdeers
        for (int i = target.Count - 1; i >= 0; i--)
        {
            shapeGoals.Remove(target[i]);
            goalHolder.Remove(target[i]);
        }

        //Remove same goals from all siblings
        for (int i = target.Count - 1; i >= 0; i--)
        {
            foreach(CellElement item in allSiblings)
                item.RemoveGoal(target[i]);
        }
    }

    private void ClearAll()
    {
        //Dots
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                CellElement target = cells[y * 7 + x];
                target.SetActiveState(true);

                target.RemoveDot();
                target.RemovePiece();
                target.RemoveGoal();
            }
        }

        piecesSavedCells.Clear();

        //Pieces
        if (pieces.Count > 0)
        {
            for (int i = pieces.Count - 1; i >= 0; i--)
                pieceHolder.Remove(pieces[i]);
        }

        //Goals
        if (shapeGoals.Count > 0)
        {
            for (int i = shapeGoals.Count - 1; i >= 0; i--)
                goalHolder.Remove(shapeGoals[i]);
        }
        shapeGoals.Clear();
    }
    #endregion

    private void ChangeButtonColor(Button targetButton)
    {
        if (targetButton == choosenButton)
            return;

        if(choosenButton == null)
        {
            choosenButton = targetButton;
            choosenButton.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
        }
        else
        {
            choosenButton.style.backgroundColor = new Color(0.345f, 0.345f, 0.345f);
            choosenButton = targetButton;
            choosenButton.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
        }
    }
}
