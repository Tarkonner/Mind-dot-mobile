using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeGoal : MonoBehaviour
{
    [SerializeField] public GoalDot[] goalSpecifications;

    private int goalsizeX;
    private int goalsizeY;
    private bool completed = false;

    private List<Cell> cellList = new List<Cell>();

    private Image background;
    private Color uncompletedColor;
    private Color completedColor;

    // Start is called before the first frame update
    void Start()
    {
        int goalMaxX = (int)goalSpecifications[0].gridPos.x;
        int goalMaxY = (int)goalSpecifications[0].gridPos.x;
        int goalMinX = (int)goalSpecifications[0].gridPos.y;
        int goalMinY = (int)goalSpecifications[0].gridPos.y;
        for (int i = 1; i < goalSpecifications.Length; i++)
        {
            goalMaxX = Mathf.Max(goalMaxX, (int)goalSpecifications[i].gridPos.x);
            goalMinX = Mathf.Min(goalMinX, (int)goalSpecifications[i].gridPos.x);

            goalMaxY = Mathf.Max(goalMaxY, (int)goalSpecifications[i].gridPos.y);
            goalMinY = Mathf.Min(goalMinY, (int)goalSpecifications[i].gridPos.y);
        }
        goalsizeX = goalMaxX - goalMinX + 1;
        goalsizeY = goalMaxY - goalMinY + 1;
        
        RectTransform goalRect = gameObject.GetComponent<RectTransform>();
        //The dots need to be scaled according to how many can fit within the square
        //The +1 is to account for the 0-indexed grid position.
        int detSize = Mathf.Max(new int[] { goalsizeX, goalsizeY});
        foreach (GoalDot goalDot in goalSpecifications)
        {
            RectTransform dotRect = goalDot.GetComponent<RectTransform>();
            dotRect.sizeDelta = goalRect.sizeDelta/detSize;
            Debug.Log(goalDot.gridPos);
            //Debug.Log(new Vector2(goalsizeX / 2, goalsizeY / 2) * dotRect.sizeDelta); 
            dotRect.anchoredPosition = goalRect.anchoredPosition +(goalDot.gridPos-
                new Vector2(goalsizeX/2-0.5f*((goalsizeX+1)%2),goalsizeY/2-0.5f * ((goalsizeY+1) % 2))) * dotRect.sizeDelta;
        }

        background = GetComponent<Image>();
        if (uncompletedColor==Color.clear) 
        {
            uncompletedColor = new Color(0.75f, 0.75f, 0.75f, 1);
        }
        if (completedColor==Color.clear)
        {
            completedColor = new Color(0.1f, 0.9f, 0.2f, 1);
        }
        background.color = uncompletedColor;
    }

    public bool CheckFulfilment(Board board)
    {
        for (int x = 0; x < board.grid.GetLength(0); x++)
        {
            for (int y = 0; y < board.grid.GetLength(1); y++)
            {
                if (board.grid[x, y] == null) { continue; }

                if (board.grid[x,y].occupying is Dot currentDot)
                {
                    if(currentDot.dotType == goalSpecifications[0].dotType)
                    {
                        Debug.Log(goalSpecifications[0].dotType);
                        if (CheckForPatternAtPosition(board,new Vector2(x, y)))
                        {
                            completed = true;
                            background.color = completedColor;
                            return true;
                        }
                    }
                }
            }
        }
        completed = false;
        background.color=uncompletedColor;
        return false;
    }
    private bool CheckForPatternAtPosition(Board board, Vector2 currentPos) 
    {
        cellList.Clear();
        for (int i = 0; i < goalSpecifications.Length; i++)
        {
            Vector2 assumedPos = new Vector2(currentPos.x, currentPos.y) + (goalSpecifications[i].gridPos - goalSpecifications[0].gridPos);
            if ((int)assumedPos.x > board.grid.GetLength(0)-1 || (int)assumedPos.y > board.grid.GetLength(1)-1) { return false; }
            if (board.grid[(int)assumedPos.x, (int)assumedPos.y] == null) { return false; }

            if (board.grid[(int)assumedPos.x, (int)assumedPos.y].occupying is Dot checkDot &&
                checkDot.dotType == goalSpecifications[i].dotType)
            {
                cellList.Add(board.grid[(int)assumedPos.x, (int)assumedPos.y]);
            }
            else
            {
                return false;
            }
        }
        //Act on cellList
        foreach (var cell in cellList)
        {
            Debug.Log($"Pattern found in: {cell}");
        }
        return true;
    }
}
