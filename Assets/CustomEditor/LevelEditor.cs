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
    private List<CellElement> piecesSavedCells = new List<CellElement>();
    VisualElement pieceHolder;
    List<PieceElement> pieces = new List<PieceElement>();
    //Goals
    private List<CellElement> goalSavedCells = new List<CellElement>();
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
        //Goals
        leftPanel.Add(new Label("Goals"));
        leftPanel.Add(new Button(() => { editTypeIndex = 6; }) { text = "Mark goal dots" });
        leftPanel.Add(new Button(() => { MakeGoal(); }) { text = "TestShapeGoal" });
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
                if (!cellElement.turnedOff && cellElement.childCount > 0)
                    return;
                cellElement.SetDot(new DotElement(DotType.Red));
                break;
            case 2:
                if (!cellElement.turnedOff && cellElement.childCount > 0)
                    return;
                cellElement.SetDot(new DotElement(DotType.Blue));
                break;
            case 3:
                if (!cellElement.turnedOff && cellElement.childCount > 0)
                    return;
                cellElement.SetDot(new DotElement(DotType.Yellow));
                break;
            //Remove
            case 4:
                cellElement.RemoveDot();
                break;

            //Pieces
            case 5:
                if (cellElement.partOfPiece || cellElement.partOfGoal)
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
                if (cellElement.partOfPiece || cellElement.partOfGoal)
                    break;

                if(!goalSavedCells.Contains(cellElement))
                {
                    goalSavedCells.Add(cellElement);
                    cellElement.tintColor = Color.yellow;
                }
                else
                {
                    goalSavedCells.Remove(cellElement);
                    cellElement.SetDefultColor();
                }
                break;
        }
    }

    private void MakePiece()
    {
        if (piecesSavedCells.Count > 0)
        {
            //Change back color
            for (int i = 0; i < piecesSavedCells.Count; i++)
                piecesSavedCells[i].tintColor = Color.white;


            //Remove cells without dots
            for (int i = piecesSavedCells.Count - 1; i >= 0; i--)
            {
                if (piecesSavedCells[i].holding == null)
                    piecesSavedCells.RemoveAt(i);
            }

            if (piecesSavedCells.Count == 0)
                return;

            PieceElement pieceElement = new PieceElement();
            pieces.Add(pieceElement);

            //Calculate position
            Vector2Int lowPoint = new Vector2Int(10, 10);
            Vector2Int highPoint = new Vector2Int(0, 0);
            for (int i = 0; i < piecesSavedCells.Count; i++)
            {
                //Low
                if (lowPoint.x > piecesSavedCells[i].gridCoordinates.x)
                    lowPoint.x = piecesSavedCells[i].gridCoordinates.x;
                if (lowPoint.y > piecesSavedCells[i].gridCoordinates.y)
                    lowPoint.y = piecesSavedCells[i].gridCoordinates.y;

                //High
                if (highPoint.x < piecesSavedCells[i].gridCoordinates.x)
                    highPoint.x = piecesSavedCells[i].gridCoordinates.x;
                if (highPoint.y < piecesSavedCells[i].gridCoordinates.y)
                    highPoint.y = piecesSavedCells[i].gridCoordinates.y;
            }

            for (int i = 0; i < piecesSavedCells.Count; i++)
            {
                //Set refence point
                Vector2Int targetCoor = piecesSavedCells[i].gridCoordinates - new Vector2Int(lowPoint.x, lowPoint.y);

                pieceElement.AddDot(targetCoor, piecesSavedCells[i].holding);

                //Change background color
                piecesSavedCells[i].SetPiece(pieceElement);
            }

            pieceElement.Construct();
            pieceElement.style.marginRight = new StyleLength(10); // Add right margin
            pieceElement.style.marginBottom = new StyleLength(10); // Add bottom margin
            pieceHolder.Add(pieceElement);

            piecesSavedCells.Clear();
        }
        else
            Debug.Log("No cells chosen");
    }

    private void MakeGoal()
    {
        //Make object
        ShapeGoalElement shapeGoalElement = new ShapeGoalElement();
        shapeGoals.Add(shapeGoalElement);
        goalHolder.Add(shapeGoalElement);

        shapeGoalElement.SetGridSize(new Vector2Int(2, 2));
        shapeGoalElement.Construct();
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
        if(shapeGoals.Count > 0)
        {
            for (int i = shapeGoals.Count - 1; i >= 0; i--)
                goalHolder.Remove(shapeGoals[i]);
        }
    }
}
