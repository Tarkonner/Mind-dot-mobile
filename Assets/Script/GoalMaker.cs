using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalMaker : ScaleAnimations
{
    [SerializeField] GameObject placeGoalCounter;
    [SerializeField] protected GameObject placeGoalPrefab;
    
    [SerializeField] GameObject shapeGoalPrefab;
    public Transform holder;

    [Header("Animations")]
    [SerializeField] private float animateInTime = .5f;
    [SerializeField] private float animateOutTime = .5f;
    private List<GameObject> goalsToAnimate = new List<GameObject>();



    public void MakeGoals(LevelSO levelGoals)
    {
        //Leave exits
        if (holder.transform.childCount > 0)
        {
            ScaleOutLiniar(goalsToAnimate, animateOutTime);
            StartCoroutine(MakeLevels(animateOutTime, levelGoals));
        }
        else
            StartCoroutine(MakeLevels(0, levelGoals));
    }

    IEnumerator MakeLevels(float waitTime, LevelSO levelGoals)
    {
        yield return new WaitForSeconds(waitTime + .05f);
        //Remove old goals
        if (holder.transform.childCount > 0)
        {
            for (int i = holder.transform.childCount - 1; i >= 0; i--)
                Destroy(holder.transform.GetChild(i).gameObject);

            goalsToAnimate.Clear();
        }

        //Placement goal
        if (levelGoals.levelPlaceGoals.Length > 0)
        {
            GameObject counterSpawn = Instantiate(placeGoalCounter, holder);
            CountPlacementGoals countPlacementGoals = counterSpawn.GetComponent<CountPlacementGoals>();
            goalsToAnimate.Add(counterSpawn);

            //Placement goals on Board
            foreach (var item in levelGoals.levelPlaceGoals)
            {
                GameObject spawn = Instantiate(placeGoalPrefab);
                PlaceGoal pg = spawn.GetComponent<PlaceGoal>();
                pg.MakeGoal(item, Board.Instance.grid);
                countPlacementGoals.AddToGoalsToCheck(pg);
                //Transform
                spawn.transform.SetParent(Board.Instance.grid[pg.cell.gridPos.x, pg.cell.gridPos.y].transform);
                spawn.transform.localScale = Vector3.one;
            }
        }

        //Shape goals
        for (int i = 0; i < levelGoals.levelShapeGoals.Length; i++)
        {
            GameObject spawnedGoal = Instantiate(shapeGoalPrefab, holder);
            ShapeGoal shapeGoal = spawnedGoal.GetComponent<ShapeGoal>();
            shapeGoal.LoadGoal(levelGoals.levelShapeGoals[i]);

            goalsToAnimate.Add(spawnedGoal);
        }

        ScaleInLiniar(goalsToAnimate, animateInTime);
    }
}
