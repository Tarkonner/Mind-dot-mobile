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


    [MenuItem("Tools/Level Editor")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<LevelEditor>();
        wnd.titleContent = new GUIContent("Level Editor");
    }


    public void CreateGUI()
    {

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
        leftPanel.Add(new Button(() => { editTypeIndex = 1; }) { text = "Red Dot" });
        leftPanel.Add(new Button(() => { editTypeIndex = 2; }) { text = "Blue Dot" });
        leftPanel.Add(new Button(() => { editTypeIndex = 3; }) { text = "Yellow Dot" });
        leftPanel.Add(new Button(() => { editTypeIndex = 4; }) { text = "Remove Dot" });
        //Pieces
        leftPanel.Add(new Label("Pieces"));
        leftPanel.Add(new Button(() => { editTypeIndex = 5; }) { text = "Mark piece dots" });
        leftPanel.Add(new Button(() => { MakePiece(); }) { text = "Make piece" });
        leftPanel.Add(new Button(() => { editTypeIndex = 9; }) { text = "Change rotation setting"}) ;
        leftPanel.Add(new Button(() => { editTypeIndex = 7; }) { text = "Remove piece" });
        //Goals
        leftPanel.Add(new Label("Goals"));
        leftPanel.Add(new Button(() => { editTypeIndex = 6; }) { text = "Mark shape goal dots" });
        leftPanel.Add(new Button(() => { MakeShapeGoal(); }) { text = "Make Shape Goal" });
        leftPanel.Add(new Button(() => { editTypeIndex = 8; }) { text = "Remove Shape Goal" });
        //Board
        leftPanel.Add(new Label("Board"));
        leftPanel.Add(new Button(() => { ClearAll(); }) { text = "Clear board" });


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
                    target.ChangeShowSprite(false);

                    target.RemoveDot();
                }
                else
                    target.ChangeShowSprite(true);
            }
        }

        //Reset
        horizontal.value = 0;
        vertical.value = 0;
    }

    public void OnCellClicked(CellElement cellElement)
    {
        switch (editTypeIndex)
        {
            //Cells
            case 0:
                cellElement.ChangeShowSprite();
                break;

            //Dots
            //Placement
            case 1:
                if (cellElement.turnedOff || cellElement.childCount > 0)
                    return;
                cellElement.SetDot(new DotElement(DotType.Red));
                break;
            case 2:
                if (cellElement.turnedOff || cellElement.childCount > 0)
                    return;
                cellElement.SetDot(new DotElement(DotType.Blue));
                break;
            case 3:
                if (cellElement.turnedOff || cellElement.childCount > 0)
                    return;
                cellElement.SetDot(new DotElement(DotType.Yellow));
                break;
            //Remove
            case 4:
                cellElement.RemoveDot();
                break;

            //Pieces
            case 5:
                if (cellElement.partOfPiece)
                    break;

                if (!piecesSavedCells.Contains(cellElement))
                {
                    piecesSavedCells.Add(cellElement);
                    cellElement.tintColor = Color.cyan;
                }
                else
                {
                    piecesSavedCells.Remove(cellElement);
                    cellElement.SetDefultColor();
                }
                break;

            //Goals
            case 6:
                if (!goalSavedCells.Contains(cellElement))
                {
                    goalSavedCells.Add(cellElement);
                    cellElement.tintColor = Color.gray;
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
                {
                    pieces.Remove((PieceElement)targetGrid);
                    pieceHolder.Remove(targetGrid);

                }
                break;


            //Remove goal
            case 8:
                if(targetGrid is ShapeGoalElement)
                {
                    shapeGoals.Remove((ShapeGoalElement)targetGrid);
                    goalHolder.Remove(targetGrid);
                }
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
                targetElements[i].tintColor = Color.white;


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

            return spawnedGrid;
        }
        else
        {
            Debug.Log("No cells chosen");
            return null;
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
                target.ChangeShowSprite(true);

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
    }
}
