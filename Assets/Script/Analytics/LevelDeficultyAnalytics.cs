using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

public class LevelDeficultyAnalytics : MonoBehaviour
{
    public delegate void OnQuestionerComplet();
    public static event OnQuestionerComplet onQuestionerComplet;

    private void SendData(string difficultyText)
    {
        Debug.Log(difficultyText);

        CustomEvent levelDifficultyEvent = new CustomEvent("LevelDifficulty")
        {
            {"0", DataBetweenLevels.Instance.targetLevel },
            {"1", difficultyText }
        };
        AnalyticsService.Instance.RecordEvent(levelDifficultyEvent);

        onQuestionerComplet?.Invoke();

        gameObject.SetActive(false);
    }

    public void EasyFun()
    {
        SendData("EF");
    }

    public void EasyBoring()
    {
        SendData("EB");
    }

    public void HardFun()
    {
        SendData("HF");
    }

    public void HardBoring()
    {
        SendData("HB");
    }
}
