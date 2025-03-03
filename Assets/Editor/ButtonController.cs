using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonController
{
    LevelEditor levelEditor;

    //Change Button color
    private Button choosenButton;
    private VisualElement root;

    private DotType placeDotType;
    public DotType PlaceDotType { get { return placeDotType; } }

    public ButtonController(VisualElement rootElement, LevelEditor levelEditor)
    {
        root = rootElement;
        this.levelEditor = levelEditor;
    }

    public void ChangeButtonColor(Button targetButton)
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

    public Clickable SelectetAction(string name)
    {
        Button targetButton = root.Q<Button>(name);
        targetButton.clickable.clicked += () => ChangeButtonColor(targetButton);
        return targetButton.clickable;
    }

    public Clickable ButtonAction(string name)
    {
        return root.Q<Button>(name).clickable;
    }

    public void DotButton(Button targetButton, DotType targetDotType, Color targetColor)
    {
        targetButton.clickable.clicked += () => { levelEditor.stateMachine.ChangeState(new PlaceDotState()); placeDotType = targetDotType; ChangeButtonColor(targetButton); };
        Image dotImage = new Image();
        dotImage.sprite = Resources.Load<Sprite>("Circle");
        dotImage.tintColor = targetColor;
        targetButton.Add(dotImage);
    }
}
