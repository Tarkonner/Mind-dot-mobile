using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EditorStateMachine
{
    public EditorStats editorStats;
    private LevelEditor levelEditor;

    public EditorStateMachine(LevelEditor levelEditor)
    {
        this.levelEditor = levelEditor;
    }

    public void ActiveState(EditorStats targetState, VisualElement visualElement = null)
    {
        if(editorStats != targetState)
        {
            editorStats.Exit();
            editorStats = targetState;
            editorStats.Enter(levelEditor);
        }

        editorStats.Execute(visualElement);
    }
}
