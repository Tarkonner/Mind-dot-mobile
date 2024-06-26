using SharedData;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelEditor : EditorWindow
{
    //Style
    public VisualTreeAsset styleSheet;
    public VisualTreeAsset eo_PieceHolder;
    public VisualTreeAsset eo_GoalHolder;

    //Editor
    VisualElement grid;
    public List<CellElement> cells { get; private set; } = new List<CellElement>();
    private DotType placeDotType;

    //Cell
    SliderInt horizontalSlider;
    SliderInt verticalSlider;
    //Save and load
    TextField namingField;
    ObjectField inputtedLevelField;

    //State machine
    private EditorState currentState = new CellEditState();

    //Pieces
    VisualElement pieceHolder;
    public List<PieceElement> piecesData = new List<PieceElement>();
    //Goals
    VisualElement goalHolder;
    public List<ShapeGoalElement> shapeGoals = new List<ShapeGoalElement>();
    public List<CellElement> placeGoalCells = new List<CellElement>();

    //Change Button color
    private Button choosenButton;

    [MenuItem("Tools/Level Editor")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<LevelEditor>();
        wnd.titleContent = new GUIContent("Level Editor");
    }


    private void ChangeState(EditorState targetState)
    {
        if(currentState.GetType() != targetState.GetType())
        {
            currentState.Exit();
            currentState = targetState;
            currentState.Enter();
        }
    }

    public void OnEnable()
    {
        rootVisualElement.Add(styleSheet.Instantiate());

        //Options
        ButtonAction("ResetEditor").clicked += ClearAll;

        //Cells
        horizontalSlider = rootVisualElement.Q("HorizontalValue") as SliderInt;
        verticalSlider = rootVisualElement.Q("VerticalValue") as SliderInt;
        ButtonAction("ResizeGrid").clicked      += () => ResizeGrid(new Vector2(horizontalSlider.value, verticalSlider.value));
        //Set start marked
        SelectetAction("CellActivation").clicked  += () => ChangeState(new CellEditState());

        //Dots
        DotButton(rootVisualElement.Q<Button>("RedDot"), DotType.Red, Color.red);
        DotButton(rootVisualElement.Q<Button>("BlueDot"), DotType.Blue, Color.blue);
        DotButton(rootVisualElement.Q<Button>("YellowDot"), DotType.Yellow, new Color(247, 255, 0));

        //Pieces
        pieceHolder = rootVisualElement.Q("PieceScroller");
        SelectetAction("ChoosePieceCells").clicked += () => ChangeState(new MakePieceState());
        ButtonAction("MakePiece").clicked += () => 
            { if (currentState is MakePieceState) ((MakePieceState)currentState).Execute(pieceHolder, eo_PieceHolder, this); };

        //Goal
        //Shape goals
        goalHolder = rootVisualElement.Q("GoalHolder");
        SelectetAction("ChooseShapeGoalCells").clicked += () => ChangeState(new MakeShapeGoalState());
        ButtonAction("MakeShapeGoal").clicked += () =>
            { if (currentState is MakeShapeGoalState) ((MakeShapeGoalState)currentState).Execute(goalHolder, eo_GoalHolder, this); };
        //Placement goals
        SelectetAction("MakePlaceGoal").clicked += () => ChangeState(new MakePlaceGoalState());

        //Save and load
        namingField = rootVisualElement.Q("LevelsName") as TextField;
        inputtedLevelField = rootVisualElement.Q("LoadLevelField") as ObjectField;
        inputtedLevelField.RegisterValueChangedCallback((evt) =>
        {
            if(inputtedLevelField.value != null)
                rootVisualElement.Q<Button>("SaveLevel").text = "Override level";
            else
                rootVisualElement.Q<Button>("SaveLevel").text = "Save";
        });
        ButtonAction("SaveLevel").clicked += SaveOrOverride;
        ButtonAction("LoadLevel").clicked += LoadLevel;

        //Grid
        grid = rootVisualElement.Q("GridHolder");
        grid.style.flexDirection = FlexDirection.Column;

        CellElement blueprint = new CellElement(Vector2Int.zero, this);
        int gridSize = 7;

        // Add cells to the grid
        // Create a new row element and add it to the grid
        VisualElement row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row; // Set the row to align horizontally
        grid.Add(row);
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                var cellElement = new CellElement(new Vector2Int(j, i), this);

                // Add the cell to the current row
                row.Add(cellElement);

                // If the row is full, add it to the grid and start a new row
                if ((j + 1) % gridSize == 0)
                {
                    grid.Add(row); // Add the full row to the grid
                    row = new VisualElement(); // Create a new row
                    row.style.flexDirection = FlexDirection.Row; // Set the row to align horizontally
                }

                cells.Add(cellElement);
            }
        }
    }

    void DotButton(Button targetButton, DotType targetDotType, Color targetColor)
    {
        targetButton.clickable.clicked += () => { ChangeState(new PlaceDotState()); placeDotType = targetDotType; ChangeButtonColor(targetButton); };
        Image dotImage = new Image();
        dotImage.sprite = Resources.Load<Sprite>("Circle");
        dotImage.tintColor = targetColor;
        targetButton.Add(dotImage);
    }

    void ChangeButtonColor(Button targetButton)
    {
        if (targetButton == choosenButton)
            return;

        if (choosenButton == null)
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

    private Clickable SelectetAction(string name)
    {
        Button targetButton = rootVisualElement.Q<Button>(name);
        targetButton.clickable.clicked += () => ChangeButtonColor(targetButton);
        return targetButton.clickable;
    }

    private Clickable ButtonAction(string name)
    {
        return rootVisualElement.Q<Button>(name).clickable;
    }

    public void OnCellClicked(CellElement cellElement, int buttonIndex)
    {
        switch (currentState)
        {
            case CellEditState:
                ((CellEditState)currentState).Execute(cellElement);
                break;
            case PlaceDotState:
                ((PlaceDotState)currentState).Execute(placeDotType, buttonIndex, cellElement);
                break;
            case MakePieceState:
                ((CollectCells)currentState).AddCell(cellElement, CellColorState.choosenPiece);
                break;
            case MakeShapeGoalState:
                ((CollectCells)currentState).AddCell(cellElement, CellColorState.choosenGoal);
                break;
            case MakePlaceGoalState:
                ((MakePlaceGoalState)currentState).Execute(cellElement, buttonIndex, this);
                break;
        }
    }

    private void ResizeGrid(Vector2 targetSize)
    {
        //Clamp
        targetSize = new Vector2(Math.Clamp(horizontalSlider.value, 1, 7), Math.Clamp(verticalSlider.value, 1, 7));

        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                CellElement target = cells[y * 7 + x];

                if (targetSize.x <= x || targetSize.y <= y)
                {
                    target.SetActiveState(false);
                    target.RemoveDot();
                }
                else
                    target.SetActiveState(true);
            }
        }
    }

    
    private void PlaceDot(Vector2Int coordinats, DotType type)
    {
        cells[coordinats.y * 7 + coordinats.x].SetDot(new DotElement(type));
    }



    public void RemovePiecesDots(PieceElement target)
    {
        for (int i = target.siblings.Count - 1; i >= 0; i--)
        {            
            target.siblings[i].RemovePiece();
        }

        pieceHolder.Remove(target.holder);
    }

    public void RemoveGoal(ShapeGoalElement target)
    {
        for (int i = 0; i < target.siblings.Count; i++)
        {
            target.siblings[i].RemoveGoal();
        }
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

        //Remove same goals from all siblings
        for (int i = 0; i < target.Count; i++)
        {
            foreach (CellElement cell in allSiblings)
                cell.RemoveGoal(target[i]);
        }

        //Remove from holdeers
        for (int i = target.Count - 1; i >= 0; i--)
        {
            goalHolder.Remove(target[i].holder);
        }
    }

    private void ClearAll()
    {
        //Pieces
        for (int i = pieceHolder.childCount - 1; i >= 0; i--)
        {
            VisualElement child = pieceHolder[i];
            pieceHolder.Remove(child);
        }
        piecesData.Clear();

        //Goals
        for (int i = goalHolder.childCount - 1; i >= 0; i--)
        {
            VisualElement child = goalHolder[i];
            goalHolder.Remove(child);
        }
        shapeGoals.Clear();

        //Dots
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                CellElement target = cells[y * 7 + x];
                target.SetActiveState(true);

                target.RemoveDot();
                target.SetDefaultColor();
                target.RemovePlacementGoal();
            }
        }
    }

    #region Save
    private void SaveOrOverride()
    {
        if (inputtedLevelField.value == null)
            SaveLevelToSO();
        else
        {
            LevelSO level = (LevelSO)inputtedLevelField.value;

            //Cell
            List<CellData> cellDatas = new List<CellData>();
            foreach (CellElement item in cells)
                cellDatas.Add(item.cellData);
            //Piece
            LevelPiece[] pieces = new LevelPiece[piecesData.Count];
            for (int i = 0; i < piecesData.Count; i++)
            {
                pieces[i] = new LevelPiece(piecesData[i].gridData as PieceData);
            }
            //Shape goals
            LevelShapeGoal[] shape = new LevelShapeGoal[shapeGoals.Count];
            for (int i = 0; i < shapeGoals.Count; i++)
            {
                shape[i] = new LevelShapeGoal(shapeGoals[i].gridData);
            }
            //Placement goals
            LevelPlaceGoal[] place = new LevelPlaceGoal[placeGoalCells.Count];
            for (int i = 0; i < placeGoalCells.Count; i++)
            {
                place[i] = new LevelPlaceGoal(placeGoalCells[i].cellData.gridCoordinates, placeGoalCells[i].cellData.holding.dotType);
            }

            level.OverrideLevel(new LevelBoard(cellDatas, GridSize()), pieces, shape, place);

            Debug.Log("Overridede level");
        }
    }
    private void SaveLevelToSO()
    {
        //Legal level
        if(piecesData.Count == 0)
        {
            Debug.Log("No Pieces in level");
            return;
        }
        if(shapeGoals.Count == 0 && placeGoalCells.Count == 0)
        {
            Debug.Log("No Goals");
            return;
        }

        //Set typed name
        string levelName = "";
        if(namingField.value != null)
            levelName = namingField.value.ToString();

        //Converter
        //Cell
        List<CellData> cellDatas = new List<CellData>();
        foreach (CellElement item in cells)
            cellDatas.Add(item.cellData);
        //Piece
        List<PieceData> pieceDatas = new List<PieceData>();
        foreach (PieceElement data in piecesData)
            pieceDatas.Add(data.gridData as PieceData);
        //Shape goals
        List<GridData> gridDatas = new List<GridData>();
        foreach (GridElement item in shapeGoals)
            gridDatas.Add(item.gridData);
        //Placement goals
        List<PlaceGoalElement> placeGoals = new List<PlaceGoalElement>();
        foreach (var cell in placeGoalCells)
            placeGoals.Add(cell.placeGoal);
        List<PlaceGoalData> placeGoalDatas = new List<PlaceGoalData>();
        foreach (PlaceGoalElement item in placeGoals)
            placeGoalDatas.Add(item.placeGoalData);

        //Make SO
        (bool workingLevel, LevelSO SO_Level) = LevelConverter.SaveLevel(levelName, pieceDatas, cellDatas, GridSize(), gridDatas, placeGoalDatas);

        //Message statues
        if (workingLevel)
        {
            Debug.Log("Level Saved!");
            //Override
            inputtedLevelField.value = SO_Level;
        }
        else
        {
            Debug.Log("Error saving level");
            namingField.value = null;
        }
    }
    #endregion

    Vector2 GridSize()
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
    #region Load
    private void LoadLevel()
    {
        if(inputtedLevelField.value == null)
        {
            Debug.Log("No level selected");
            return;
        }

        ClearAll();

        LevelSO targetLevel = (LevelSO)inputtedLevelField.value;

        //Grid
        LevelBoard targetGrid = targetLevel.levelGrid;

        //Load Dots
        for (int y = 0; y < 7; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                //Turn out of level cells off
                if (x >= (int)targetGrid.boardSize.x ||
                    y >= (int)targetGrid.boardSize.y)
                {
                    cells[y * 7 + x].TurnOffCell();
                    continue;
                }

                int loadGridIndex = y * (int)targetGrid.boardSize.x + x;
                int editorGridIndex = y * 7 + x;

                //See if grid is atice & if there is a dot
                if (!targetGrid.activeCells[loadGridIndex])
                    cells[editorGridIndex].TurnOffCell();
                else if (targetGrid.dots[loadGridIndex] != DotType.Null)
                    PlaceDot(new Vector2Int(x, y), targetGrid.dots[loadGridIndex]);
            }
        }

        //Load Pieces
        foreach (LevelPiece item in targetLevel.levelPieces)
            LoadPiece(item);

        //Load Shape goals
        foreach (LevelShapeGoal item in targetLevel.levelShapeGoals)
            LoadShapeGoal(item);

        //Load Placement goals
        foreach(LevelPlaceGoal item in targetLevel.levelPlaceGoals)
            cells[(int)(item.goalPosition.y * 7 + item.goalPosition.x)].AddPlacementGoal(item.type);

        //Cleanup
        inputtedLevelField.value = null;
        ChangeState(new CellEditState());

        //Ready to override
        inputtedLevelField.value = targetLevel;
    }

    private void LoadPiece(LevelPiece targetPiece)
    {
        List<CellElement> result = new List<CellElement>();
        for (int i = 0; i < targetPiece.dotPositions.Length; i++)
        {
            CellElement spawnCell = new CellElement(new Vector2Int((int)targetPiece.dotPositions[i].x, (int)targetPiece.dotPositions[i].y), this);
            DotElement spawnDot = new DotElement(targetPiece.dotTypes[i]);
            spawnCell.SetDot(spawnDot);
            result.Add(spawnCell);
        }

        //State machine
        ChangeState(new MakePieceState());
        ((MakePieceState)currentState).PremakeCells(result);
        ((MakePieceState)currentState).Execute(pieceHolder, eo_PieceHolder, targetPiece, this);
    }

    private void LoadShapeGoal(LevelShapeGoal targetShapeGoal)
    {
        //Load elements
        List<CellElement> result = new List<CellElement>();
        for (int i = 0; i < targetShapeGoal.goalDots.Length; i++)
        {
            CellElement spawnCell = new CellElement(new Vector2Int((int)targetShapeGoal.goalSpecifications[i].x, (int)targetShapeGoal.goalSpecifications[i].y), this);
            DotElement spawnDot = new DotElement(targetShapeGoal.goalDots[i]);
            spawnCell.SetDot(spawnDot);
            result.Add(spawnCell);
        }

        //State machine
        ChangeState(new MakeShapeGoalState());
        ((MakeShapeGoalState)currentState).PremakeCells(result);
        ((MakeShapeGoalState)currentState).Execute(goalHolder, eo_GoalHolder, targetShapeGoal, this);
    }
    #endregion
}
