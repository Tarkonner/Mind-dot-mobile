using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EditorStats 
{
    public LevelEditor editor;

    public virtual void Enter(LevelEditor levelEditor)
    {
        editor = levelEditor;
    }

    public virtual void Execute(VisualElement target = null)
    {

    }

    public virtual void Exit()
    {

    }
}
