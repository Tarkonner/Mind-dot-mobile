using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Analytics.Internal;
using UnityEngine;
using UnityEngine.Analytics;
using static LevelManager;

public class LevelManager : MonoBehaviour
{
    [Header("Refences")]
    [SerializeField] Board board;
    [SerializeField] PieceMaker pieceHolder;
    [SerializeField] GoalMaker goalMaker;
    [SerializeField] LevelText levelText;

    [Header("Animations")]
    [SerializeField] float completedLevelPauseTime = .5f;
    [SerializeField] float sizeAnimationTime = .5f;
    [SerializeField] float dotsBonusSize = .15f;

    [Header("Goals")]
    [SerializeField] private GameObject goalHolder;
    [SerializeField] private LevelsBank levelsBank;
    private List<IGoal> allGoals = new List<IGoal>();


    [Header("Testing")]
    [SerializeField] bool loadTestlevel = false;
    [SerializeField] private LevelSO testLevel;

    [Header("Questioner")]
    [SerializeField] GameObject questioner;
    [SerializeField] bool showQuestioner = false;

    //Analytics
    private float timeForCompletingLevels;
    private bool runningLevelClock = false;
    public LevelSO currentLevel { get; private set; }

    //Events
    public delegate void OnLoadLevel();
    public static event OnLoadLevel onLoadLevel;
    public delegate void OnDeloadLevel();
    public static event OnDeloadLevel onDeloadLevel;
    public delegate void OnLevelComplete();
    public static event OnLevelComplete onLevelComplete;

    private void Start()
    {
        //Load level
#if (UNITY_EDITOR)
        if (loadTestlevel && testLevel != null)
        {
            LoadLevel(testLevel);
            return;
        }
        
        //If begining in game scene
        if(DataBetweenLevels.Instance == null)
        {
            GameObject data = new GameObject();
            data.AddComponent<DataBetweenLevels>();    
        }
#endif

        //Load first level
        LoadLevel(levelsBank.levels[DataBetweenLevels.Instance.targetLevel]);

        //UI show level index
        levelText.LevelIndex(DataBetweenLevels.Instance.targetLevel);
    }

    private void OnEnable()
    {
        InputSystem.onDotChange += GoalProgression;

        //Analytics
        onLoadLevel += () => runningLevelClock = true;
        //Questioner
        if (showQuestioner)
            LevelDeficultyAnalytics.onQuestionerComplet += LoadNextLevel;
    }
    private void OnDisable()
    {
        InputSystem.onDotChange -= GoalProgression;

        //Questioner
        if (showQuestioner)
            LevelDeficultyAnalytics.onQuestionerComplet -= LoadNextLevel;
    }

    private void Update()
    {
        if(runningLevelClock)
            timeForCompletingLevels += Time.deltaTime;
    }

    public void LoadLevel(LevelSO targetLevel)
    {
        currentLevel = targetLevel;

        onLoadLevel?.Invoke();

        //Clear old
        allGoals.Clear();

        board.LoadLevel(targetLevel); //Uses info from both board & pieces, so piece dots don't get loadet in
        goalMaker.MakeGoals(targetLevel);
        pieceHolder.MakePieces(targetLevel.levelPieces);
    }

    public void GoalProgression()
    {
        //See how many goals
        if (allGoals.Count == 0)
        {
            for (int i = 0; i < goalMaker.holder.childCount; i++)
            {
                IGoal check = goalMaker.holder.GetChild(i).GetComponent<IGoal>();

                if (check != null)
                {
                    allGoals.Add(check);
                }
            }
        }

        int completedGoals = 0;
        foreach (var child in allGoals)
        {
            if (child.CheckFulfilment(board))
            {
                completedGoals++;
            }
        }
        if (completedGoals == allGoals.Count)
        {
            //Tell test levelsBank
#if (UNITY_EDITOR)
            if (loadTestlevel)
            {
                Debug.Log("Complete level");
                return;
            }
#endif

            //Analytics
            CustomEvent levelData = new CustomEvent("LevelComplete")
            {
                {"0", DataBetweenLevels.Instance.targetLevel },
                {"1", timeForCompletingLevels }
            };
            AnalyticsService.Instance.RecordEvent(levelData);
            timeForCompletingLevels = 0;
            runningLevelClock = false;

            //Load level            
            if (DataBetweenLevels.Instance.targetLevel == levelsBank.levels.Length)
            {
                //Back to start
                Debug.Log("All levels complete");
                DataBetweenLevels.Instance.targetLevel = 0;

                onDeloadLevel?.Invoke();
                LoadLevel(levelsBank.levels[DataBetweenLevels.Instance.targetLevel]);
                levelText.LevelIndex(DataBetweenLevels.Instance.targetLevel);
            }
            else
            {
                Debug.Log("Level Complete");
                StartCoroutine(WinAnimation());
            }
        }
    }



    public void LoadNextLevel()
    {        
        //What next level is
        DataBetweenLevels.Instance.targetLevel++;

        onDeloadLevel?.Invoke(); //Event
        LoadLevel(levelsBank.levels[DataBetweenLevels.Instance.targetLevel]); //Target level
        levelText.LevelIndex(DataBetweenLevels.Instance.targetLevel); //Set level text
    }

    IEnumerator WinAnimation()
    {
        //Get Dots from completet goals
        List<Dot> collectetDots = new List<Dot>();
        for (int i = 0; i < allGoals.Count; i++)
        {
            if (allGoals[i] is ShapeGoal)
            {
                ShapeGoal sg = (ShapeGoal)allGoals[i];
                for (int j = 0; j < sg.lastCheckedDots.Count; j++)
                    collectetDots.Add(sg.lastCheckedDots[j]);
            }
        }
        //Animate
        for (int i = 0; i < collectetDots.Count; i++)
        {
            float targetScale = collectetDots[i].transform.localScale.x + dotsBonusSize;
            collectetDots[i].transform.DOScale(targetScale, sizeAnimationTime);
        }

        onLevelComplete?.Invoke();
        yield return new WaitForSeconds(completedLevelPauseTime);

        //Load next level if not showing question
        if (!showQuestioner)
            LoadNextLevel();
        else
            questioner.SetActive(true);
    }
}