using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonMaker : MonoBehaviour
{
    [Header("Placeing level buttons")]
    [SerializeField] int horizontalElements = 4;
    [SerializeField] int verticalElements = 4;
    [Header("Buttons setup")]
    [SerializeField] GameObject levelButtonPrefab;
    [SerializeField] float spaceBetweenButtons = 100;
    [SerializeField] GameObject levelHolder;
    private Image[,] outerImages;
    [Header("Panels")]
    private int numberOfPanels = 3;
    [SerializeField] GameObject leftArrowButton;
    [SerializeField] GameObject rightArrowButton;
    [SerializeField] float spaceBetweenPanels = 900;
    [SerializeField] float slideAnimationTime = .5f;
    private List<Transform> holders = new List<Transform>();
    private LevelSelectButton[,] buttonsBank;
    private int panelCount = 0;
    private int maxPanels;
    private bool panelsMoveing = false;
    [Header("Text")]
    [SerializeField] TextMeshProUGUI levelCountText;
    [Header("Color")]
    [SerializeField] ColorBank colorBank;


    public void Setup()
    {
        //Arrow buttons
        leftArrowButton.SetActive(false);

        //Level
        int targetLevel = 0;

        Vector2 spawnPoint = new Vector2(-spaceBetweenButtons * horizontalElements / 2 + spaceBetweenButtons / 2, spaceBetweenButtons * verticalElements / 2);

        //Save resused asset;
        buttonsBank = new LevelSelectButton[numberOfPanels, horizontalElements * verticalElements];
        outerImages = new Image[numberOfPanels, horizontalElements * verticalElements];

        //Make button
        for (int j = 0; j < numberOfPanels; j++)
        {
            //Holder
            GameObject hold = new GameObject($"{j} panel");
            hold.transform.SetParent(levelHolder.transform);
            hold.transform.localScale = Vector3.one;
            holders.Add(hold.transform);
            hold.transform.localPosition = new Vector2(j * spaceBetweenPanels, 0);

            //Spawn buttons
            for (int y = 0; y < verticalElements; y++)
            {
                for (int x = 0; x < horizontalElements; x++)
                {
                    int count = y * verticalElements + x;

                    //Load level when clicket
                    GameObject spawn = Instantiate(levelButtonPrefab, holders[j]);
                    LevelSelectButton lsb = spawn.GetComponent<LevelSelectButton>();
                    lsb.TargetLevel(targetLevel); //Set level and text
                    Button button = spawn.GetComponent<Button>();
                    button.onClick.AddListener(lsb.LoadLevel);
                    //Save button to later
                    buttonsBank[j, count] = lsb;

                    //Set to next level
                    targetLevel++;

                    //What color
                    switch (targetLevel % 3)
                    {
                        case 0:
                            lsb.dotType = DotType.Red;
                            break;
                        case 1:
                            lsb.dotType = DotType.Blue;
                            break;
                        case 2:
                            lsb.dotType = DotType.Yellow;
                            break;
                    }

                    //Placement
                    Vector2 targetTransform = spawnPoint + new Vector2(x * spaceBetweenButtons, y * -spaceBetweenButtons);
                    button.transform.localPosition = targetTransform;


                    outerImages[j, count] = spawn.GetComponent<Image>();
                    Image innerImage = spawn.transform.GetChild(0).gameObject.GetComponentInChildren<Image>();
                    switch (lsb.dotType)
                    {
                        case DotType.Red:
                            innerImage.color = colorBank.redColor;
                            break;
                        case DotType.Blue:
                            innerImage.color = colorBank.blueColor;
                            break;
                        case DotType.Yellow:
                            innerImage.color = colorBank.yellowColor;
                            break;
                    }
                }
            }
        }

        //Update text
        TextForCount();
    }


    public void ShowButtonOnOpening()
    {
        //Set panels
        maxPanels = (int)MathF.Ceiling((float)DataBetweenLevels.Instance.currentLevelChunk.levels.Length / (horizontalElements * verticalElements));



        //Buttons for 1 panel
        for (int y = 0; y < verticalElements; y++)
        {
            for (int x = 0; x < horizontalElements; x++)
            {
                int count = y * verticalElements + x;

                //Turn buttons on or off
                if (count >= DataBetweenLevels.Instance.currentLevelChunk.levels.Length)
                    buttonsBank[0, count].gameObject.SetActive(false);
                else
                {
                    buttonsBank[0, count].gameObject.SetActive(true);

                    //Check for save
                    bool levelcompletionState = false;
                    if (ES3.FileExists())
                    {
                        string key = SaveSystem.levelKey + DataBetweenLevels.Instance.currentLevelChunk.name + count.ToString();
                        levelcompletionState = ES3.Load<bool>(key);
                    }

                    //Give color
                    OuterRingLook(levelcompletionState, 0, count);
                }


            }
        }

        //Check if need to show arrow
        if (DataBetweenLevels.Instance.currentLevelChunk.levels.Length < verticalElements * horizontalElements)
            rightArrowButton.gameObject.SetActive(false);
        else
            rightArrowButton.gameObject.SetActive(true);
    }

    public void MovePanels(bool right)
    {
        if (panelsMoveing)
            return;
        else
            panelsMoveing = true;


        if (right)
        {
            if (panelCount + 1 == maxPanels)
                return;


            int targetPanel = (panelCount + 2) % numberOfPanels;

            //Move
            foreach (Transform target in holders)
            {
                target.DOLocalMove(target.localPosition - new Vector3(spaceBetweenPanels, 0, 0), slideAnimationTime).OnComplete(() =>
                {
                    //Move panel to back
                    holders[targetPanel].transform.localPosition = new Vector2(spaceBetweenPanels, 0);
                    panelsMoveing = false;
                });
            }

            panelCount++;
            //Updata level load
            if (panelCount > 0 && panelCount + 1 < maxPanels)
                UpdatePanelsButton(targetPanel, true);

            //Turn arrow off
            if (panelCount == maxPanels - 1)
                rightArrowButton.SetActive(false);

            leftArrowButton.SetActive(true);
        }
        else
        {
            //Stop moveing beyond 0
            if (panelCount == 0)
                return;

            //Move
            int targetPanel = (panelCount + 1) % numberOfPanels;
            foreach (Transform target in holders)
            {
                target.DOLocalMove(target.localPosition + new Vector3(spaceBetweenPanels, 0, 0), slideAnimationTime).OnComplete(() =>
                {
                    //Move to front
                    holders[targetPanel].transform.localPosition = new Vector2(-spaceBetweenPanels, 0);
                    panelsMoveing = false;
                });
            }
            panelCount--;

            //Updata level load
            if (panelCount != 0)
                UpdatePanelsButton(targetPanel, false);

            //Turn arrow off
            if (panelCount == 0)
                leftArrowButton.SetActive(false);

            rightArrowButton.SetActive(true);
        }

        int currentLevels = DataBetweenLevels.Instance.currentLevelChunk.levels.Length % (horizontalElements * verticalElements);
        //Turn button on or off
        for (int y = 0; y < verticalElements; y++)
        {
            for (int x = 0; x < horizontalElements; x++)
            {
                int count = y * verticalElements + x;

                if (currentLevels > count)
                    buttonsBank[panelCount, count].gameObject.SetActive(true);
                else
                    buttonsBank[panelCount, count].gameObject.SetActive(false);
            }
        }

        //Update text
        TextForCount();
    }

    void TextForCount()
    {
        int numberOfLevels = panelCount * horizontalElements * verticalElements;
        levelCountText.text = $"Level {numberOfLevels + 1} - {numberOfLevels + (horizontalElements * verticalElements)}";
    }

    void UpdatePanelsButton(int targetPanel, bool right)
    {
        for (int y = 0; y < verticalElements; y++)
        {
            for (int x = 0; x < horizontalElements; x++)
            {
                int count = y * verticalElements + x;

                int movementModefier = -1;
                if (right)
                    movementModefier = 1;

                int cal = ((panelCount + movementModefier) * horizontalElements * verticalElements) + count;
                LevelSelectButton targetButton = buttonsBank[targetPanel, count];
                targetButton.TargetLevel(cal);

                //Outer line
                string key = SaveSystem.levelKey + DataBetweenLevels.Instance.currentLevelChunk.name + cal.ToString();
                OuterRingLook(ES3.Load<bool>(key), (panelCount + movementModefier) % numberOfPanels, count);
            }
        }
    }

    void OuterRingLook(bool levelCompletet, int panel, int targetLevel)
    {
        if (levelCompletet)
        {
            outerImages[panel, targetLevel].color = Color.black;
        }
        else
        {
            switch (buttonsBank[panel, targetLevel].dotType)
            {
                case DotType.Red:
                    outerImages[panel, targetLevel].color = colorBank.backgroundRedColor;
                    break;
                case DotType.Blue:
                    outerImages[panel, targetLevel].color = colorBank.backgroundBlueColor;
                    break;
                case DotType.Yellow:
                    outerImages[panel, targetLevel].color = colorBank.backgroundYellowColor;
                    break;
            }
        }
    }
}
