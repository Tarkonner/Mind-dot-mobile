using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    private InterstitialAd adShower;

    //Levels
    [SerializeField] int levelsBeforeAds = 2;
    private int levelCompleted = 0;

    //Time
    [SerializeField] float TimeBeforeAdd;
    private float timeGone;

    void Awake()
    {
        adShower = GetComponent<InterstitialAd>();
    }

    private void OnEnable()
    {
        LevelManager.onDeloadLevel += AdController;
    }

    private void OnDisable()
    {
        LevelManager.onDeloadLevel -= AdController;
    }

    private void Update()
    {
        timeGone += Time.deltaTime;
    }

    void AdController()
    {
        if(timeGone >= TimeBeforeAdd)
            adShower.ShowAd();
        else
            CountUp();
    }

    public void CountUp()
    {
        levelCompleted++;
        if (levelCompleted == levelsBeforeAds)
            adShower.ShowAd();
    }
}
