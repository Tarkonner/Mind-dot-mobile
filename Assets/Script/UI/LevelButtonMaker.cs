using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonMaker : MonoBehaviour
{
    [SerializeField] LevelsBank levelsBank;
    [SerializeField] GameObject levelButton;
    [SerializeField] float spaceBetweenButtons = 100;
    [SerializeField] GameObject levelHolder;
    private List<Transform> holders = new List<Transform>();

    void Start()
    {
        //Get holder
        foreach (Transform child in levelHolder.transform)
        {
            holders.Add(child);
        }

        //Level
        int targetLevel = 0;

        Vector2 spawnPoint = new Vector2(-spaceBetweenButtons * 2f, spaceBetweenButtons * 2f);

        //Spawn 30 buttons
        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 5; x++)
            {

                //Load level when clicket
                GameObject spawn = Instantiate(levelButton, holders[0]);
                LevelSelectButton lsb = spawn.GetComponent<LevelSelectButton>();
                lsb.targetLevel = targetLevel;
                Button button = spawn.GetComponent<Button>();
                button.onClick.AddListener(() => lsb.LoadLevel());


                //Set to next level
                targetLevel++;

                //Placement
                Vector2 targetTransform = spawnPoint + new Vector2(x * spaceBetweenButtons, y * -spaceBetweenButtons);
                button.transform.localPosition = targetTransform;

                //Text
                TextMeshProUGUI textElement = button.GetComponentInChildren<TextMeshProUGUI>();
                textElement.text = targetLevel.ToString();
            }

        }
    }
}
