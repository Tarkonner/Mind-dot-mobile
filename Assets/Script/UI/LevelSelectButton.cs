using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelectButton : MonoBehaviour
{
    public int targetLevel = 0;

    private TextMeshProUGUI levelText;

    public DotType dotType;

    private void Awake()
    {
        levelText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void TargetLevel(int targetLevel)
    {
        if(levelText == null)
            levelText = GetComponentInChildren<TextMeshProUGUI>();

        levelText.text = (targetLevel + 1).ToString();
        this.targetLevel = targetLevel;
    }

    public void LoadLevel()
    {
        DataBetweenLevels.Instance.targetLevel = targetLevel;
        SceneController.Instance.LoadScene(2);
    }
}
