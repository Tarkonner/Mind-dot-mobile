using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountPlacementGoals : MonoBehaviour, IGoal
{
    [SerializeField] ColorBank colorBank;
    [SerializeField] TextMeshProUGUI textBox;
    private List<PlaceGoal> subGoals = new List<PlaceGoal>();
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.color = colorBank.uncompleteGoal;
    }

    public void AddToGoalsToCheck(PlaceGoal goal)
    {
        subGoals.Add(goal);
        textBox.text = $"0 / {subGoals.Count}";
    }

    public bool CheckFulfilment(Board board)
    {
        int dotPlaced = 0;
        for (int i = 0; i < subGoals.Count; i++)
        {
            if(subGoals[i].CheckFulfilment())
                dotPlaced++;
        }

        textBox.text = $"{dotPlaced} / {subGoals.Count}";

        if (dotPlaced == subGoals.Count)
        {
            image.color = colorBank.completeGoal;
            return true;
        }
        else
        {
            image.color = colorBank.uncompleteGoal;
            return false;
        }
    }
}
