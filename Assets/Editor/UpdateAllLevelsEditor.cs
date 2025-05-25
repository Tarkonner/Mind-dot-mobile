using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UpdateAllLevelsEditor : EditorWindow
{
    [MenuItem("Tools/Update All")]
    public static void ShowWindow()
    {
        EditorWindow wnd = GetWindow<UpdateAllLevelsEditor>();
        wnd.titleContent = new GUIContent("Update All");
        wnd.minSize = new Vector2(400, 300);
    }

    private void OnEnable()
    {
        rootVisualElement.Add(CreateUpdateUI());
    }

    private VisualElement CreateUpdateUI()
    {
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Column;
        container.style.alignItems = Align.Stretch;

        // Add update button
        var updateButton = new Button(UpdateAllLevels)
        {
            text = "Update All Levels"
        };
        updateButton.style.marginBottom = 10;
        container.Add(updateButton);

        // Add progress indicator
        var progress = new ProgressBar { value = 0 };
        container.Add(progress);

        // Add status label
        var statusLabel = new Label();
        container.Add(statusLabel);

        return container;
    }

    private async void UpdateAllLevels()
    {
        string[] guids = AssetDatabase.FindAssets("t:LevelSO");
        int totalLevels = guids.Length;
        int currentLevel = 0;

        // Get the main editor window
        LevelEditor mainEditor = EditorWindow.GetWindow<LevelEditor>();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            LevelSO levelSO = AssetDatabase.LoadAssetAtPath<LevelSO>(assetPath);

            if (levelSO != null)
            {
                EditorUtility.SetDirty(levelSO);
                mainEditor.LoadAndResaveObject(levelSO);
                AssetDatabase.SaveAssets();

                // Update progress
                float progress = (float)++currentLevel / totalLevels;
                rootVisualElement.Q<ProgressBar>().value = progress;
                rootVisualElement.Q<Label>().text =
                    $"Processing level {currentLevel} of {totalLevels}";
            }

            await Task.Yield();
        }

        rootVisualElement.Q<Label>().text = "Update complete!";
    }
}
