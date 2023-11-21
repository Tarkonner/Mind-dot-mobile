using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGoal : MonoBehaviour
{
    [SerializeField] public GoalDot[] goalSpecifications;

    private int goalsizeX;
    private int goalsizeY;
    private bool completed = false;

    private List<Cell> cellList;

    // Start is called before the first frame update
    void Start()
    {
        goalsizeX = (int)goalSpecifications[0].gridPos.x;
        goalsizeY = (int)goalSpecifications[0].gridPos.y;
        for (int i = 1; i < goalSpecifications.Length; i++)
        {
            if (goalsizeX < goalSpecifications[i].gridPos.x)
            {
                goalsizeX = (int)goalSpecifications[i].gridPos.x;
            }
            if (goalsizeY < goalSpecifications[i].gridPos.y)
            {
                goalsizeY = (int)goalSpecifications[i].gridPos.y;
            }
        }
    }

    bool CheckFulfilment(Board board)
    {
        for (int x = 0; x < board.grid.GetLength(1)-goalsizeX; x++)
        {
            for (int y = 0; y < board.grid.GetLength(2)-goalsizeY; y++)
            {
                if (board.grid[x,y].occupying is Dot currentDot)
                {
                    if(currentDot.dotType == goalSpecifications[0].dotType)
                    {
                        if (CheckForPatternAtPosition(board,new Vector2(x, y)))
                        {
                            completed = true;
                            return true;
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
        }
        return false;
    }
    private bool CheckForPatternAtPosition(Board board, Vector2 currentPos) 
    {
        for (int i = 1; i < goalSpecifications.Length; i++)
        {
            cellList.Clear();
            Vector2 assumedPos = new Vector2(currentPos.x, currentPos.y) + goalSpecifications[i].gridPos;
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
        return true;
    }
}
