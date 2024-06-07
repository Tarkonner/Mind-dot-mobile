using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonMaker : MonoBehaviour
{
    [SerializeField] LevelsBank levelsBank;
    [Header("Buttons setup")]
    [SerializeField] GameObject levelButtonPrefab;
    [SerializeField] float spaceBetweenButtons = 100;
    [SerializeField] GameObject levelHolder;
    [Header("Panels")]
    [SerializeField] GameObject leftArrowButton;
    [SerializeField] GameObject rightArrowButton;
    [SerializeField] float spaceBetweenPanels = 900;
    [SerializeField] float slideAnimationTime = .5f;
    private List<Transform> holders = new List<Transform>();
    private int panelCount = 0;
    private int maxPanels;
    [Header("Text")]
    [SerializeField] TextMeshProUGUI textMesh;
    [Header("Color")]
    [SerializeField] ColorBank colorBank;

    void Start()
    {
        maxPanels = (int)MathF.Ceiling((float)levelsBank.levels.Length / 30f) - 1;

        //Level
        int targetLevel = 0;

        Vector2 spawnPoint = new Vector2(-spaceBetweenButtons * 2f, spaceBetweenButtons * 2f);

        for (int j = 0; j < 3; j++)
        {
            //Holder
            GameObject hold = new GameObject($"{targetLevel} - {targetLevel + 30}");
            hold.transform.SetParent(levelHolder.transform);
            hold.transform.localScale = Vector3.one;
            holders.Add(hold.transform);
            hold.transform.localPosition = new Vector2(j * spaceBetweenPanels, 0);

            //Spawn 30 buttons
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    //No level made
                    if (targetLevel > levelsBank.levels.Length)
                        break;

                    //Load level when clicket
                    GameObject spawn = Instantiate(levelButtonPrefab, holders[j]);
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

                    //Give color
                    Image image = spawn.GetComponent<Image>();
                    switch (targetLevel % 3)
                    {
                        case 0:
                            image.color = colorBank.redColor;
                            break;
                        case 1:
                            image.color = colorBank.blueColor; 
                            break;
                        case 2:
                            image.color = colorBank.yellowColor; 
                            break;
                    }
                }
            }
        }
    }

    public void MovePanels(bool right)
    {       
        if (right)
        {
            if (panelCount == maxPanels)
                return;

            //Move
            foreach(Transform target in holders)
            {
                target.DOLocalMove(target.localPosition - new Vector3(spaceBetweenPanels, 0, 0), slideAnimationTime);
            }

            panelCount++;

            //Turn arrow off
            if (panelCount == maxPanels)
                rightArrowButton.SetActive(false);
            
            leftArrowButton.SetActive(true);
        }
        else
        {
            //Stop moveing beond 0
            if (panelCount == 0)
                return;

            //See if moveing to first panel


            //Move
            foreach (Transform target in holders)
            {
                target.DOLocalMove(target.localPosition + new Vector3(spaceBetweenPanels, 0, 0), slideAnimationTime);
            }

            panelCount--;

            //Turn arrow off
            if(panelCount == 0)
                leftArrowButton.SetActive(false);

            rightArrowButton.SetActive(true);
        }

        //Update text
        textMesh.text = $"Level {holders[panelCount].name}";
    }
}
