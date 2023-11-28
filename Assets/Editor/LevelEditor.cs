using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelEditor : EditorWindow
{
    //Editor
    VisualElement rightPanel;
    VisualElement grid;

    int editTypeIndex = 0;

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
        var leftPane = new ListViewContainer();
        rightPanel = new ListViewContainer();
        splitView.Add(leftPane);
        splitView.Add(rightPanel);

        // Add buttons to the left pane
        leftPane.Add(new Button(() => { editTypeIndex = 0; }) { text = "Turn cells on & off" });
        leftPane.Add(new Button(() => { editTypeIndex = 1; }) { text = "Button 2" });

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
                cell.ConvertToCell(new SerializableCell { gridPos = new Vector2Int(i, j) });
                var cellElement = new CellElement(cell, new Vector2Int(i, j), this);

                //Set spacing
                cellElement.style.marginRight = new StyleLength(spaceing / 2);
                cellElement.style.marginTop = new StyleLength(spaceing);

                grid.Add(cellElement);
            }
        }

        // Add the grid to the right pane
        rightPanel.Add(grid);
    }

    public void OnCellClicked(CellElement cellElement)
    {
        switch (editTypeIndex)
        {
            case 0:
                cellElement.ChangeShowSprite();
                break;

            case 1:
                if (cellElement.childCount > 1)
                    return;

                DotElement dotElement = new DotElement(DotType.Red);

                cellElement.Add(dotElement);

                dotElement.style.left = (cellElement.resolvedStyle.width - dotElement.resolvedStyle.width) / 2;
                dotElement.style.top = (cellElement.resolvedStyle.height - dotElement.resolvedStyle.height) / 2;

                break;
        }
    }
}
