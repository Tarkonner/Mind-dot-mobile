using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelectButton : MonoBehaviour
{
    public int targetLevel = 0;

    private TextMeshProUGUI levelText;

    private void Awake()
    {
        levelText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void TargetLevel(int targetLevel)
    {
        levelText.text = (targetLevel + 1).ToString();
    }

    public void LoadLevel()
    {
        DataBetweenLevels.Instance.targetLevel = targetLevel;
        SceneController.Instance.LoadScene(1);
    }
}
