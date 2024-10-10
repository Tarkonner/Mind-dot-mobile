using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayButtonUI : MonoBehaviour
{
    //Components
    TextMeshProUGUI buttonText;

    [SerializeField] LevelsBank[] levelsBanks;

    private LevelsBank targetBank;
    private bool fromTheBeginning = false;

    // Start is called before the first frame update
    void Awake()
    {
        if(!ES3.FileExists())
        {
            Debug.Log("No Save file on startup");
            return;
        }

        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        UpdateButton();
    }

    private void OnEnable()
    {
        UpdateButton();
    }

    void UpdateButton()
    {
        string targetLevelChunk = ES3.Load<string>("SelectLevelChunk");

        //Have begun a chunk
        if (targetLevelChunk == "begin")
        {
            buttonText.text = "Begin";
            fromTheBeginning = true;
        }
        else
        {
            buttonText.text = "Resume";
        }

        foreach (LevelsBank item in levelsBanks)
        {
            if (item.name == targetLevelChunk)
                targetBank = item;
        }
    }

    public void BeginPlay()
    {
        if(!fromTheBeginning)
        {
            if(targetBank != null)
            {
                DataBetweenLevels.Instance.currentLevelChunk = targetBank;
                int foundTargetLevel = ES3.Load<int>(targetBank.name) + 1;
                DataBetweenLevels.Instance.targetLevel = foundTargetLevel;
            }
            else
            {
                DataBetweenLevels.Instance.currentLevelChunk = levelsBanks[2];
                DataBetweenLevels.Instance.targetLevel = 0;
            }         
        }
        else
        {
            fromTheBeginning = false;

            //Load the toturial
            DataBetweenLevels.Instance.currentLevelChunk = levelsBanks[0];
            DataBetweenLevels.Instance.targetLevel = 0;
        }

        SceneController.Instance.LoadScene(2);
    }
}
