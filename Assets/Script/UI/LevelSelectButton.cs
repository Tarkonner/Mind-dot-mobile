using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectButton : MonoBehaviour
{
    public int targetLevel = 0;

    public void LoadLevel()
    {
        DataBetweenLevels.Instance.targetLevel = targetLevel;
        SceneController.Instance.LoadScene(1);
    }
}
