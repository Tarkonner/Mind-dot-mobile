using log4net.Core;
using SharedData;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelEditor : EditorWindow
{
    public VisualElement mainRoot { get; private set; }

    //Style
    public VisualTreeAsset styleSheet;
    public VisualTreeAsset eo_PieceHolder;
    public VisualTreeAsset eo_GoalHolder;

    //Editor    
    public InteractiveGrid interactiveGrid;   
   
    //SaveWithField and load
    TextField namingField;
    ObjectField inputtedLevelField;

    //State machine
    public EditorStateMachine stateMachine = new EditorStateMachine();
    
    //Pieces
    VisualElement pieceHolder;
    public List<PieceElement> piecesData = new List<PieceElement>();
    //Goals
    VisualElement goalHolder;
    public List<ShapeGoalElement> shapeGoals = new List<ShapeGoalElement>();
    public List<CellElement> placeGoalCells = new List<CellElement>();

    ButtonController buttonController;


    [MenuItem("Tools/Level Editor")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<LevelEditor>();
        wnd.titleContent = new GUIContent("Level Editor");
    }

    public void OnEnable()
    {
        rootVisualElement.Add(styleSheet.Instantiate());
        mainRoot = rootVisualElement;

        //Button
        buttonController = new ButtonController(rootVisualElement, this);

        //Options
        buttonController.ButtonAction("ResetEditor").clicked += ClearAll;

        //Cells
        buttonController.ButtonAction("ResizeGrid").clicked += () => interactiveGrid.ResizeGrid();
        //Set start marked
        buttonController.SelectetAction("CellActivation").clicked  += () => stateMachine.ChangeState(new CellEditState());

        //Level generation
        buttonController.ButtonAction("RandomLevelGeneration").clicked += () => interactiveGrid.MakeRandomCells();
        buttonController.ButtonAction("PlaceRandomDots").clicked += () => FillLevelWithDots();

        //Dots
        buttonController.DotButton(rootVisualElement.Q<Button>("RedDot"),    DotType.Red, Color.red);
        buttonController.DotButton(rootVisualElement.Q<Button>("BlueDot"),   DotType.Blue, Color.blue);
        buttonController.DotButton(rootVisualElement.Q<Button>("YellowDot"), DotType.Yellow, new Color(247, 255, 0));

        //Pieces
        pieceHolder = rootVisualElement.Q("PieceScroller");
        buttonController.SelectetAction("ChoosePieceCells").clicked += () => stateMachine.ChangeState(new MakePieceState());
        buttonController.ButtonAction("MakePiece").clicked += () => 
        { 
            if (stateMachine.CurrentState is MakePieceState) 
                ((MakePieceState)stateMachine.CurrentState).Execute(pieceHolder, eo_PieceHolder, this); 
        };

        //Goal
        //Shape goals
        goalHolder = rootVisualElement.Q("GoalHolder");
        buttonController.SelectetAction("ChooseShapeGoalCells").clicked += () => stateMachine.ChangeState(new MakeShapeGoalState());
        buttonController.ButtonAction("MakeShapeGoal").clicked += () =>
        { 
            if (stateMachine.CurrentState is MakeShapeGoalState) 
                ((MakeShapeGoalState)stateMachine.CurrentState).Execute(goalHolder, eo_GoalHolder, this); 
        };
        //Placement goals
        buttonController.SelectetAction("MakePlaceGoal").clicked += () => stateMachine.ChangeState(new MakePlaceGoalState());

        //SaveWithField and load
        namingField = rootVisualElement.Q("LevelsName") as TextField;
        inputtedLevelField = rootVisualElement.Q("LoadLevelField") as ObjectField;
        inputtedLevelField.RegisterValueChangedCallback((evt) =>
        {
            if(inputtedLevelField.value != null)
                rootVisualElement.Q<Button>("SaveLevel").text = "Override level";
            else
                rootVisualElement.Q<Button>("SaveLevel").text = "Save";
        });
        buttonController.ButtonAction("SaveLevel").clicked += SaveWithField;
        buttonController.ButtonAction("LoadLevel").clicked += LoadLevelFromField;

        //Grid
        VisualElement grid = rootVisualElement.Q("GridHolder");
        interactiveGrid = new InteractiveGrid(grid, this);
    }

    public void OnCellClicked(CellElement cellElement, int buttonIndex)
    {
        switch (stateMachine.CurrentState)
        {
            case CellEditState:
                ((CellEditState)stateMachine.CurrentState).Execute(cellElement);
                break;
            case PlaceDotState:
                ((PlaceDotState)stateMachine.CurrentState).Execute(buttonController.PlaceDotType, buttonIndex, cellElement);
                break;
            case MakePieceState:
                ((CollectCells)stateMachine.CurrentState).AddCell(cellElement, CellColorState.choosenPiece);
                break;
            case MakeShapeGoalState:
                ((CollectCells)stateMachine.CurrentState).AddCell(cellElement, CellColorState.choosenGoal);
                break;
            case MakePlaceGoalState:
                ((MakePlaceGoalState)stateMachine.CurrentState).Execute(cellElement, buttonIndex, this);
                break;
        }
    }

    #region Dot placement
    private void PlaceDot(Vector2Int coordinats, DotType type)
    {
        interactiveGrid.cells[coordinats.y * InteractiveGrid.gridSize + coordinats.x].SetDot(new DotElement(type));
    }

    //Give a DotType without null
    private DotType RandomDotType()
    {
        //Get random dot type (Without null)
        Array values = Enum.GetValues(typeof(DotType));
        return (DotType)values.GetValue(UnityEngine.Random.Range(1, values.Length));
    }

    //Place dot on board
    void FillLevelWithDots()
    {
        float placeDotChance = .75f;
        foreach (CellElement cell in interactiveGrid.cells)
        {
            if ((float)UnityEngine.Random.Range(0f, 1f) < placeDotChance)
                cell.SetDot(new DotElement(RandomDotType()));
        }
    }
    #endregion

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
        for (int x = 0; x < InteractiveGrid.gridSize; x++)
        {
            for (int y = 0; y < InteractiveGrid.gridSize; y++)
            {
                CellElement target = interactiveGrid.cells[y * InteractiveGrid.gridSize + x];
                target.SetActiveState(true);

                target.RemoveDot();
                target.SetDefaultColor();
                target.RemovePlacementGoal();
            }
        }
    }



    #region Save
    /// <summary>
    /// Makes a new SO with the data.
    /// </summary>
    private void SaveWithField()
    {
        if (inputtedLevelField.value == null)
            SaveLevelToSO();
        else
        {
            LevelSO level = (LevelSO)inputtedLevelField.value;

            OverrideSaveLogic(level);

            Debug.Log("Overrided level");
        }
    }

    private void OverrideSaveLogic(LevelSO targetLevel)
    {
        // Collect data
        List<CellData> cellDatas = GetCellData();
        LevelPiece[] pieces = GetLevelPieces();
        LevelShapeGoal[] shape = GetShapeGoals();
        LevelPlaceGoal[] place = GetPlacementGoals();

        targetLevel.LevelOverride(new LevelBoard(cellDatas, interactiveGrid.GridSize()), pieces, shape, place,
            LevelMetadata.PiecesMetadataCalculation(piecesData, interactiveGrid));
    }

    private void SaveLevelToSO()
    {
        #region Legal level rules
        if (piecesData.Count == 0)
        {
            Debug.Log("No Pieces in level");
            return;
        }
        if(piecesData.Count > 4)
        {
            Debug.Log("To many pieces");
            return;
        }
        if(shapeGoals.Count == 0 && placeGoalCells.Count == 0)
        {
            Debug.Log("No Goals");
            return;
        }
        if (shapeGoals.Count > 3 && placeGoalCells.Count != 0)
        {
            Debug.Log("Too many Shape goals with placement goals");
            return;
        }
        if (shapeGoals.Count > 4)
        {
            Debug.Log("Too many Shape goals");
            return;
        }
        #endregion

        //Set typed name
        string levelName = "";
        if(namingField.value != null)
            levelName = namingField.value.ToString();

        // Collect data
        List<CellData> cellDatas = GetCellData();
        List<PieceData> pieceDatas = GetPieceData();
        List<GridData> gridDatas = GetShapeGridData();
        List<PlaceGoalData> placeGoalDatas = GetPlacementGoalData();

        //Make SO
        
        (bool workingLevel, LevelSO SO_Level) = LevelConverter.SaveLevel(
            levelName, pieceDatas, cellDatas, 
            interactiveGrid.GridSize(), gridDatas, placeGoalDatas,
            LevelMetadata.PiecesMetadataCalculation(piecesData, interactiveGrid));

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

    #region Collect data
    public List<CellData> GetCellData()
    {
        List<CellData> cellDatas = new List<CellData>();
        foreach (CellElement item in interactiveGrid.cells)
            cellDatas.Add(item.cellData);
        return cellDatas;
    }

    public LevelPiece[] GetLevelPieces()
    {
        LevelPiece[] pieces = new LevelPiece[piecesData.Count];
        for (int i = 0; i < piecesData.Count; i++)
            pieces[i] = new LevelPiece(piecesData[i].gridData as PieceData);
        return pieces;
    }

    public List<PieceData> GetPieceData()
    {
        List<PieceData> pieceDatas = new List<PieceData>();
        foreach (PieceElement data in piecesData)
            pieceDatas.Add(data.gridData as PieceData);
        return pieceDatas;
    }

    public LevelShapeGoal[] GetShapeGoals()
    {
        LevelShapeGoal[] shape = new LevelShapeGoal[shapeGoals.Count];
        for (int i = 0; i < shapeGoals.Count; i++)
            shape[i] = new LevelShapeGoal(shapeGoals[i].gridData);
        return shape;
    }

    public List<GridData> GetShapeGridData()
    {
        List<GridData> gridDatas = new List<GridData>();
        foreach (GridElement item in shapeGoals)
            gridDatas.Add(item.gridData);
        return gridDatas;
    }

    public LevelPlaceGoal[] GetPlacementGoals()
    {
        LevelPlaceGoal[] place = new LevelPlaceGoal[placeGoalCells.Count];
        for (int i = 0; i < placeGoalCells.Count; i++)
        {
            place[i] = new LevelPlaceGoal(
                placeGoalCells[i].cellData.gridCoordinates,
                placeGoalCells[i].placeGoal.placeGoalData.goalType);
        }
        return place;
    }

    public List<PlaceGoalData> GetPlacementGoalData()
    {
        List<PlaceGoalData> placeGoalDatas = new List<PlaceGoalData>();
        foreach (var cell in placeGoalCells)
        {
            placeGoalDatas.Add(cell.placeGoal.placeGoalData);
        }
        return placeGoalDatas;
    }
    #endregion


    #region Load
    private void LoadLevelFromField()
    {
        if(inputtedLevelField.value == null)
        {
            Debug.Log("No level selected");
            return;
        }

        ClearAll();

        LevelSO targetLevel = (LevelSO)inputtedLevelField.value;

        LoadLevelLogic(targetLevel);

        //Ready to override
        inputtedLevelField.value = targetLevel;
    }

    private void LoadLevelLogic(LevelSO targetLevel)
    {
        //Grid
        LevelBoard targetGrid = targetLevel.levelGrid;

        //Load Dots
        for (int y = 0; y < InteractiveGrid.gridSize; y++)
        {
            for (int x = 0; x < InteractiveGrid.gridSize; x++)
            {
                //Turn out of targetLevel cells off
                if (x >= (int)targetGrid.boardSize.x ||
                    y >= (int)targetGrid.boardSize.y)
                {
                    interactiveGrid.cells[y * InteractiveGrid.gridSize + x].TurnOffCell();
                    continue;
                }

                int loadGridIndex = y * (int)targetGrid.boardSize.x + x;
                int editorGridIndex = y * InteractiveGrid.gridSize + x;

                //See if grid is atice & if there is a dot
                if (!targetGrid.activeCells[loadGridIndex])
                    interactiveGrid.cells[editorGridIndex].TurnOffCell();
                else if (targetGrid.dots[loadGridIndex] != DotType.Null)
                    PlaceDot(new Vector2Int(x, y), targetGrid.dots[loadGridIndex]);
            }
        }

        //Load Placement goals
        foreach (LevelPlaceGoal item in targetLevel.levelPlaceGoals)
        {
            var cell = interactiveGrid.cells[
                (int)(item.goalPosition.y * InteractiveGrid.gridSize + item.goalPosition.x)];

            cell.AddPlacementGoal(item.type);

            if (!placeGoalCells.Contains(cell))
                placeGoalCells.Add(cell);
        }

        //Load Pieces
        foreach (LevelPiece item in targetLevel.levelPieces)
            LoadPiece(item);

        //Load Shape goals
        foreach (LevelShapeGoal item in targetLevel.levelShapeGoals)
            LoadShapeGoal(item);
               
        //Cleanup
        inputtedLevelField.value = null;
        stateMachine.ChangeState(new CellEditState());
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
        stateMachine.ChangeState(new MakePieceState());
        ((MakePieceState)stateMachine.CurrentState).PremakeCells(result);
        ((MakePieceState)stateMachine.CurrentState).Execute(pieceHolder, eo_PieceHolder, targetPiece, this);
    }

    private void LoadShapeGoal(LevelShapeGoal targetShapeGoal)
    {
        //Load elements
        Dictionary<CellElement, DotType> dataParing = new();

        for (int i = 0; i < targetShapeGoal.goalDots.Length; i++)
        {
            CellElement foundCell = interactiveGrid.cells[(int)((targetShapeGoal.goalSpecifications[i].y + targetShapeGoal.gridPosRef.y) * InteractiveGrid.gridSize + (targetShapeGoal.goalSpecifications[i].x + targetShapeGoal.gridPosRef.x))];
            dataParing.Add(foundCell, targetShapeGoal.goalDots[i]);
        }

        //State machine
        stateMachine.ChangeState(new MakeShapeGoalState());
        ((MakeShapeGoalState)stateMachine.CurrentState).MakeWithLoadData(dataParing, goalHolder, eo_GoalHolder, this);
    }

    /// <summary>
    /// So other helper classes can load and resave a targetLevel with data
    /// </summary>
    /// <returns></returns>
    public void LoadAndResaveObject(LevelSO targetLevel)
    {
        ClearAll();
        LoadLevelLogic(targetLevel);
        OverrideSaveLogic(targetLevel);
    }

    #endregion
}
