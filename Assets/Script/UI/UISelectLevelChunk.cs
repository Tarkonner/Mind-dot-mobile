using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectLevelChunk : MonoBehaviour
{
    [SerializeField] LevelButtonMaker levelButtonMaker;

    private void Awake()
    {
        levelButtonMaker.Setup();
    }

    public void GiveLevelChunk(LevelsBank levelChunk)
    {
        DataBetweenLevels.Instance.currentLevelChunk = levelChunk;        
        levelButtonMaker.ShowButtonOnOpening();
    }
}
